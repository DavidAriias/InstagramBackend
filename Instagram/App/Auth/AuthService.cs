using Instagram.App.UseCases.Types.Shared;
using Instagram.config.helpers;
using Instagram.Domain.Entities.User;
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
            // Buscar al usuario según el método de autenticación especificado.
            var user = await GetUserAsync(identifier, method);

            // Verificar si el usuario existe y si la contraseña es válida.
            if (user is null || !EncryptHelper.VerifyPassword(pass, user.Password))
            {
                // Devolver un error si el usuario no existe o la contraseña es incorrecta.
                return AuthTypeOut.CreateError("Credentials aren't valid", HttpStatusCode.BadRequest);
            }

            // Buscar el Refresh Token asociado al usuario.
            string? refreshToken = await _tokenSqlRepository.FindUserIdAsync(user.Id);

            // Si no se encuentra un Refresh Token, generarlo y guardarlo.
            if (refreshToken is null)
            {
                refreshToken = _jwtService.GenerateRefreshToken(user.Id.ToString());

                // Crear un nuevo Access Token.
                string token = _jwtService.GenerateAccessToken(user.Id.ToString());

                // Crear un objeto de respuesta exitosa que incluya los tokens.
                var auth = AuthTypeOut.CreateSuccess(
                    user.Id,
                    token,
                    3600,       // Duración del Access Token en segundos (1 hora).
                    refreshToken,
                    2592000,     // Duración del Refresh Token en segundos (30 días).
                    HttpStatusCode.Accepted
                );

                // Mapear el objeto de respuesta a una entidad y almacenar el Refresh Token.
                var authDb = AuthMapper.MapAuthTypeOutToAuthEntity(auth);
                bool isSaved = await _tokenSqlRepository.AddRefreshTokenAsync(authDb);

                // Devolver la respuesta exitosa si el Refresh Token se almacenó correctamente.
                if (isSaved)
                {
                    return auth;
                }
                else
                {
                    // Devolver un error si no se pudo almacenar el Refresh Token.
                    return AuthTypeOut.CreateError("We can't save your auth, try later", HttpStatusCode.InternalServerError);
                }
            }
            else
            {
                // Si ya existe un Refresh Token, devolver la respuesta exitosa sin guardarlo nuevamente.
                return AuthTypeOut.CreateSuccess(
                    user.Id,
                    _jwtService.GenerateAccessToken(user.Id.ToString()),
                    3600,       // Duración del Access Token en segundos (1 hora).
                    refreshToken,
                    2592000,
                    HttpStatusCode.OK// Duración del Refresh Token en segundos (30 días).
                );
            }
        }

        private async Task<UserEntity?> GetUserAsync(string identifier, AuthMethod method)
        {
            // Método privado para buscar al usuario según el método de autenticación especificado.
            // Dependiendo del método, buscar por correo electrónico, nombre de usuario, número de teléfono, o ID de usuario.
            // Retorna null si no se encuentra el usuario.
            return method switch
            {
                AuthMethod.Email => await _userRepository.FindUserByEmail(identifier),
                AuthMethod.Username => await _userRepository.FindUserByUsername(identifier),
                AuthMethod.PhoneNumber => await _userRepository.FindUserByPhoneNumber(identifier),
                AuthMethod.UserId => await _userRepository.FindUserById(Guid.Parse(identifier)),
                _ => null
            };
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

        public async Task<AuthTypeOut> CheckStatus(AuthTypeIn auth)
        {
            bool isValidToken = _jwtService.IsTokenValid(auth.Token!);
            bool isValidRefreshToken = _jwtService.IsTokenValid(auth.RefreshToken);

            var accessTokenDuration = 3600; // 1 hora
            var refreshTokenDuration = 2592000; // 30 días
            if (isValidToken)
            {
                return AuthTypeOut.CreateSuccess(
                    auth.UserId,
                    auth.Token!,
                    accessTokenDuration,
                    auth.RefreshToken,
                    refreshTokenDuration,
                    HttpStatusCode.NotModified);
            }


            var authDb = AuthMapper.MapAuthTypeInToAuthEntity(auth);
            var userId = await _tokenSqlRepository.FindRefreshTokenAsync(authDb);

            if (userId == Guid.Empty)
            {
                return AuthTypeOut.CreateError("Token refresh is not valid", HttpStatusCode.Unauthorized);
            }

            if (!isValidRefreshToken)
            {
                var newRefreshToken = _jwtService.GenerateRefreshToken(userId.ToString());
               

                return AuthTypeOut.CreateSuccess(userId, auth.Token!, 0, newRefreshToken, refreshTokenDuration, HttpStatusCode.OK);
            }

  
            var accessToken = _jwtService.GenerateAccessToken(userId.ToString());
            

            return AuthTypeOut.CreateSuccess(userId, accessToken, accessTokenDuration, auth.Token!, 2592000, HttpStatusCode.OK);
        }


    }
}
