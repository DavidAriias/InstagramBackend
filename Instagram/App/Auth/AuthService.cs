using Instagram.App.UseCases.Types.Shared;
using Instagram.config.helpers;
using Instagram.Domain.Repositories.Interfaces.Auth;
using Instagram.Domain.Repositories.Interfaces.SQL.Token;
using Instagram.Domain.Repositories.Interfaces.SQL.User;
using Instagram.Infraestructure.Mappers.Auth;
using System.Net;

namespace Instagram.App.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserSQLDbRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly ITokenSQLDbRepository _tokenSqlRepository;
        public AuthService(
            IUserSQLDbRepository userRepository,
            IJwtService jwtService,
            ITokenSQLDbRepository tokenSqlRepository)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _tokenSqlRepository = tokenSqlRepository;
        }

        public async Task<AuthTypeOut> AuthenticateAsync(string identifier, string pass, AuthMethod method)
        {
            var user = method switch
            {
                AuthMethod.Email => await _userRepository.FindUserByEmail(identifier),
                AuthMethod.Username => await _userRepository.FindUserByUsername(identifier),
                AuthMethod.PhoneNumber => await _userRepository.FindUserByPhoneNumber(identifier),
                AuthMethod.UserId => await _userRepository.FindUserById(Guid.Parse(identifier)),
                _ => null
            };

            if (user is null || !EncryptHelper.VerifyPassword(pass, user.Password))
            {
                // Si el usuario no existe o la contraseña es incorrecta, devuelve null.
                return AuthTypeOut.CreateError("Credentials aren't valid");
            }

          
            // Si el usuario y la contraseña son válidos, genera el token JWT.
            string token = _jwtService.GenerateAccessToken(user.Id.ToString());
            string refreshToken = _jwtService.GenerateRefreshToken(user.Id.ToString());

            var auth = AuthTypeOut.CreateSuccess(
                user.Id,
                token,
                3600,
                refreshToken,
                2592000
                );

            var authDb = AuthMapper.MapAuthTypeOutToAuthEntity(auth);


            bool isSaved = await _tokenSqlRepository.AddRefreshTokenAsync(authDb);

            if (isSaved)
            {
                return auth;
            } else
            {
                return AuthTypeOut.CreateError("We can't save your auth, try later");
            }

            
        }

        public async Task<ResponseType<string>> CloseSession(AuthTypeIn auth)
        {
            var authToDb = AuthMapper.MapAuthTypeInToAuthEntity(auth);
            bool isDeleted = await _tokenSqlRepository.DeleteRefreshTokenAsync(authToDb);

            if (isDeleted) return ResponseType<string>.CreateSuccessResponse(
                null,
                HttpStatusCode.NoContent
                ,"Session was closed succesfully.");

            return ResponseType<string>.CreateErrorResponse(
                System.Net.HttpStatusCode.InternalServerError,
                "Error while closing session, try again"
                );
        }

        public async Task<AuthTypeOut> GetNewAccessToken(AuthTypeIn auth)
        {
            var authDb = AuthMapper.MapAuthTypeInToAuthEntity(auth);
            // Obtiene el UserId asociado al token de refresco.
            var userId = await _tokenSqlRepository.FindUserIdAsync(authDb);

            if (userId == Guid.Empty)
            {
                return AuthTypeOut.CreateError("Refresh token isn't valid");
            }

            // Genera un nuevo token de acceso.
            string accessToken = _jwtService.GenerateAccessToken(userId.ToString()!);

            // Puedes especificar la duración del token de acceso, aquí se usa 3600 segundos (1 hora) como en tu ejemplo.
            long accessTokenDuration = 3600;

            // Puedes devolver el nuevo token de acceso.
            return AuthTypeOut.CreateSuccess(userId, accessToken, accessTokenDuration, auth.Token, 2592000);
        }


        public async Task<AuthTypeOut> GetNewRefreshToken(AuthTypeIn auth)
        {
            var authDb = AuthMapper.MapAuthTypeInToAuthEntity(auth);
            // Obtiene el UserId asociado al token de refresco.
            var userId = await _tokenSqlRepository.FindUserIdAsync(authDb);

            if (userId == Guid.Empty)
            {
                return AuthTypeOut.CreateError("Refresh token isn't valid");
            }

            string newRefreshToken = _jwtService.GenerateRefreshToken(userId.ToString()!);

            long newRefreshTokenDuration = 2592000;

            return AuthTypeOut.CreateSuccess(userId, "", 0, newRefreshToken, newRefreshTokenDuration);
        }
    }
}
