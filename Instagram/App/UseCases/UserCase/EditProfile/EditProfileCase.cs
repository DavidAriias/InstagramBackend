using Instagram.App.UseCases.Types.Shared;
using Instagram.App.UseCases.UserCase.Types;
using Instagram.config.helpers;
using Instagram.Domain.Enums;
using Instagram.Domain.Repositories.Interfaces.Blob;
using Instagram.Domain.Repositories.Interfaces.Graph.User;
using Instagram.Domain.Repositories.Interfaces.SQL.User;
using Instagram.Infraestructure.Mappers.User;
using System.Net;

namespace Instagram.App.UseCases.UserCase.EditProfile
{
    public class EditProfileCase : IEditProfileCase
    {
        private readonly IUserNeo4jRepository _neo4JRepository;
        private readonly IUserSQLDbRepository _sqlDbRepository;
        private readonly IImageBlobService _imageService;
        public EditProfileCase(
            IUserNeo4jRepository neo4JRepository, 
            IUserSQLDbRepository sQLDbRepository,
            IImageBlobService imageService)
        {
            _neo4JRepository = neo4JRepository;
            _sqlDbRepository = sQLDbRepository;
            _imageService = imageService;
        }
        public async Task<ResponseType<string>> UpdateBiography(string bio, Guid userId)
        {
            try
            {
                // Intenta realizar la actualización de la biografía.
                bool isUpdated = await _sqlDbRepository.UpdateBiography(bio, userId);

                // Define el mensaje de respuesta según si la actualización tuvo éxito o no.
                string responseMessage = isUpdated
                    ? "Your biography has been updated successfully."
                    : "Failed to update biography";

                if (isUpdated)
                {
                    // Si la actualización fue exitosa, devuelve una respuesta exitosa con el mensaje.
                    return ResponseType<string>.CreateSuccessResponse(
                        bio,
                        HttpStatusCode.OK,
                        responseMessage);
                }

                // Si la actualización falló, devuelve una respuesta de error.
                return ResponseType<string>.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    responseMessage
                    );
            }
            catch (Exception ex)
            {
                // En caso de una excepción no controlada, devuelve una respuesta de error.
                return ResponseType<string>.CreateErrorResponse(
                    HttpStatusCode.InternalServerError,
                    ex.Message
                    );
            }
        }


        public async Task<ResponseType<string>> UpdateBirthday(DateOnly birthday, Guid userId)
        {
            // Obtener la fecha de cambio más reciente de cumpleaños del usuario
            var lastChangedDate = await _sqlDbRepository.GetLastBirthdayChangeDate(userId);

            // Verificar si se puede cambiar la fecha de cumpleaños
            bool canBeChanged = BirthdayHelper.CanChangeBirthday(lastChangedDate);

            if (canBeChanged)
            {
                // Realizar la actualización de la fecha de cumpleaños
                await _sqlDbRepository.UpdateBirthday(birthday, userId);

                return ResponseType<string>.CreateSuccessResponse(
                    birthday.ToString(),
                    HttpStatusCode.OK,
                    "Your birthday has been updated successfully."
                    );
            }

            return ResponseType<string>.CreateErrorResponse(
                 HttpStatusCode.Conflict,
                 "You changed your birthday less than 3 days ago. Please try again later."
                 );
        }

        public async Task<ResponseType<string>> UpdateImageProfile(IFormFile imageProfile, Guid userId)
        {
            if (imageProfile.Length > 0)
            {
                // Verificar si la longitud del archivo de imagen es mayor que cero.

                // Buscar la URL de la imagen de perfil actual en la base de datos.
                var currentImageProfileUrl = await _sqlDbRepository.FindImageProfileById(userId);

                string? imageProfileUrl;

                if (currentImageProfileUrl is not null)
                {
                    // Si se encontró una URL de imagen de perfil, actualizar la imagen en el servicio en la nube.
                    imageProfileUrl = await _imageService.UpdateProfileImageAsync(imageProfile, currentImageProfileUrl);
                }
                else
                {
                    // Si no se encontró una URL de imagen de perfil, cargar una nueva imagen en el servicio en la nube.
                    imageProfileUrl = await _imageService.UploadProfileImageAsync(imageProfile, userId);
                }

                // Actualizar la URL de la imagen de perfil en la base de datos.
                bool isSaved = await _sqlDbRepository.UpdateImageProfile(userId, imageProfileUrl);

                if (isSaved)
                {
                    // Crear una respuesta exitosa con el ID de usuario y un mensaje.
                    return ResponseType<string>.CreateSuccessResponse(
                        imageProfileUrl,
                        HttpStatusCode.OK,
                        "Your profile picture has been updated successfully."
                        );
                }

                return ResponseType<string>.CreateErrorResponse(
                    HttpStatusCode.InternalServerError,
                    "There was an error while updating yor profile picture"
                    );
            }

            // En caso de que la longitud del archivo sea cero o la actualización no sea exitosa, crear una respuesta de error.
            return ResponseType<string>.CreateErrorResponse(
                HttpStatusCode.BadRequest,
                "There was an issue with your profile picture. Please try again later."
                );
        }



        public async Task<ResponseType<LinkType?>> UpdateLink(LinkType link, Guid userId)
        {
            // Realizar la actualización del enlace y su título
            var linkToDb = UserMapper.MapLinkTypeToLinkEntity(link);

            bool isUpdated = await _sqlDbRepository.UpdateLink(linkToDb,userId);

            if (isUpdated)
            {
                return ResponseType<LinkType?>.CreateSuccessResponse(
                    link,
                    HttpStatusCode.OK,
                    "Link has been updated successfully.");
            }

            return ResponseType<LinkType?>.CreateErrorResponse( 
                HttpStatusCode.InternalServerError,
                "Failed to updated link");
        }


        public async Task<ResponseType<string>> UpdateName(string name, Guid userId)
        {
            // Obtener el número de cambios de nombre realizados por el usuario en los últimos 14 días
            var changes = _sqlDbRepository.GetNumberChangesOnName(userId);

            // Verificar si se supera el límite de cambios de nombre
            if (changes > 2)
            {
                return ResponseType<string>.CreateErrorResponse(
                    HttpStatusCode.Conflict,
                    "You have made 2 changes in the last 14 days. Please wait before changing your name again."
                    );
            }

            // Realizar la actualización del nombre
            bool isUpdated = await _sqlDbRepository.UpdateName(name, userId);

            if (isUpdated)
            {
                return ResponseType<string>.CreateSuccessResponse(
                    name,
                    HttpStatusCode.OK,
                    "Name has been updated successfully."
                    );
            }

            return ResponseType<string>.CreateErrorResponse(
                HttpStatusCode.InternalServerError,
                "Failed to updated name"
                );

        }

        public async Task<ResponseType<string>> UpdatePassword(string newPassword, Guid userId)
        {
            // Encriptar la nueva contraseña
            var encryptedPassword = EncryptHelper.GetPassEncrypt(newPassword);

            // Actualizar la contraseña en la base de datos
            bool isUpdated = await _sqlDbRepository.UpdatePassword(encryptedPassword, userId);

            if (isUpdated)
            {
                return ResponseType<string>.CreateSuccessResponse(
                    null,
                    HttpStatusCode.NoContent,
                    "Password has been updated successfully."
                    );
            }

            return ResponseType<string>.CreateErrorResponse(
                HttpStatusCode.InternalServerError,
                "Failed to updated passsword"
                );
        }

        public async Task<ResponseType<string>> UpdatePronoun(PronounEnum pronoun, Guid userId)
        {
            // Realizar la actualización del pronombre
            bool isUpdated = await _sqlDbRepository.UpdatePronoun(pronoun.ToString(), userId);

            if (isUpdated)
            {
                return ResponseType<string>.CreateSuccessResponse(
                    pronoun.ToString(),
                    HttpStatusCode.OK,
                    "Pronoun has been updated successfully."
                    );
            }

            return ResponseType<string>.CreateErrorResponse(
                HttpStatusCode.InternalServerError,
                "Failed to updated pronoun"
                );
        }


        public async Task<ResponseType<string>> UpdateUsername(string username, Guid userId)
        {
            // Verificar si el nombre de usuario ya está en uso
            var isUsed = await _sqlDbRepository.FindUserByUsername(username);

            // Realizar la actualización del nombre de usuario en la base de datos SQL
            bool isUpdated = await _sqlDbRepository.UpdateUsername(username, userId);

            // Realizar la actualización del nombre de usuario en la base de datos Neo4j
            await _neo4JRepository.UpdateUsername(username, userId.ToString());

            // Preparar el mensaje de acuerdo a si el nombre de usuario ya está en uso o no
            string message = (isUsed is null) ? "Your username has been updated successfully." : "The username already exists";

            if (isUpdated)
            {
                return ResponseType<string>.CreateSuccessResponse(
                    username,
                    HttpStatusCode.OK
                    ,message);
            }
            return ResponseType<string>.CreateErrorResponse(
                HttpStatusCode.InternalServerError,
                message
                );
        }

        public async Task<ResponseType<string>> UpdateIsPrivateProfile(Guid userId)
        {
            // Verificar si el usuario existe
            var isExists = await _sqlDbRepository.FindUserById(userId);

            // Realizar la actualización del estado de perfil privado en la base de datos SQL
            var isUpdated = await _sqlDbRepository.UpdateIsPrivateProfile(userId);

            if (isExists is null)
            {
                return ResponseType<string>.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    "The account doesn't exist."
                    );
            }

            return isUpdated
                ? ResponseType<string>.CreateSuccessResponse(
                    null,
                    HttpStatusCode.NoContent,
                    "The account's privacy state has been changed successfully.")
                : ResponseType<string>.CreateErrorResponse( 
                HttpStatusCode.InternalServerError,
                "The account's privacy state hasn't been changed."
                );
        }

        public async Task<ResponseType<string>> UpdateIsVerificateProfile(Guid userId)
        {
            // Verificar si el usuario existe
            var isExists = await _sqlDbRepository.FindUserById(userId);

            // Realizar la actualización del estado de verificación del perfil en la base de datos SQL
            var isUpdated = await _sqlDbRepository.UpdateIsVerificateProfile(userId);

            if (isExists is null)
            {
                return ResponseType<string>.CreateErrorResponse(
                    HttpStatusCode.NotFound,
                    "The account doesn't exist."
                    ); 
            
            }

            return isUpdated
                ? ResponseType<string>.CreateSuccessResponse(
                    null,
                    HttpStatusCode.NoContent
                    ,"The account's verification state has been changed successfully.")

                : ResponseType<string>.CreateErrorResponse(
                HttpStatusCode.InternalServerError,
                "The account's verification state hasn't been changed."
                );
        }

    }
}
