using Instagram.App.Auth;
using Instagram.App.UseCases.Types.Shared;
using Instagram.App.UseCases.UserCase.Types;
using Instagram.Domain.Repositories.Interfaces.Blob;
using Instagram.Domain.Repositories.Interfaces.Graph.User;
using Instagram.Domain.Repositories.Interfaces.SQL.Notifications;
using Instagram.Domain.Repositories.Interfaces.SQL.User;
using Instagram.Infraestructure.Mappers.User;

namespace Instagram.App.UseCases.UserCase.SignUp
{
    public class SignUpCase : ISignUpCase
    {
        private readonly IUserNeo4jRepository _neo4JRepository;
        private readonly IUserSQLDbRepository _sqlDbRepository;
        private readonly IImageBlobService _blobService;
        public SignUpCase(
            IUserSQLDbRepository sQLDbRepository,
            IUserNeo4jRepository neo4JRepository,
            INotificationsSQLDbRepository notificationRepository,
            IImageBlobService blobService) 
        {

            _sqlDbRepository = sQLDbRepository;
            _neo4JRepository = neo4JRepository;
            _blobService = blobService;
           
        }

        public async Task<ResponseType<string>> CreateUser(UserTypeIn user)
        {
            if (await _sqlDbRepository.FindUserByUsername(user.Username) is not null)
            {
                return ResponseType<string>.CreateErrorResponse(
                    System.Net.HttpStatusCode.Conflict,
                    "The username is not available"
                    );
            }

            if (user.Email is not null && await _sqlDbRepository.FindUserByEmail(user.Email) is not null)
            {
                return ResponseType<string>.CreateErrorResponse(
                    System.Net.HttpStatusCode.Conflict,
                    "The email is not available"
                    );
            }

            // Crear un objeto de dominio a partir de los datos de entrada
            var userToDb = UserMapper.MapUserTypeInToUserEntity(user);

            // Guardar el usuario en la base de datos SQL
            var result = await _sqlDbRepository.CreateUser(userToDb);

            if (result is not null)
            {
                var userId = result.Id.ToString();

                if (user.ImageProfile is not null)
                    userToDb.Imageprofile = await _blobService.UploadProfileImageAsync(user.ImageProfile, Guid.Parse(userId));

                await _neo4JRepository.CreateUser(userToDb);
                
                await _sqlDbRepository.CreateUserDescription(userToDb);

                return ResponseType<string>.CreateSuccessResponse(
                    userId, System.Net.HttpStatusCode.Created, "Welcome to Instagram");

            }

            return ResponseType<string>.CreateErrorResponse(
                System.Net.HttpStatusCode.InternalServerError,
                "There was an error while create your account, try later"
                );
        }
    }
}
