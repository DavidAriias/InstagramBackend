using Instagram.App.UseCases.MediaCases.Types.Shared.Media;
using Instagram.App.UseCases.MediaCases.Types.Shared.Media.Events;
using Instagram.App.UseCases.Types.Shared;
using Instagram.Domain.Enums;
using Instagram.Domain.Repositories.Interfaces.Document.Post;
using Instagram.Domain.Repositories.Interfaces.Document.Reel;
using Instagram.Domain.Repositories.Interfaces.SQL.User;
using Instagram.Infraestructure.Mappers.Shared.Media;
using System.Net;

namespace Instagram.App.UseCases.MediaCases.ReplyCase
{
    public class ReplyCase : IReplyCase
    {
        private readonly IPostMongoDbRepository _postRepository;
        private readonly IUserSQLDbRepository _userSQLDbRepository;
        private readonly ILogger<ReplyCase> _logger;
        private readonly IReelMongoDbRepository _reelRepository;
        public ReplyCase(IPostMongoDbRepository postRepository,
            IUserSQLDbRepository userSQLDbRepository, 
            ILogger<ReplyCase> logger,
            IReelMongoDbRepository reelRepository
            ) 
        {
            _postRepository = postRepository;
            _userSQLDbRepository = userSQLDbRepository;
            _logger = logger;
            _reelRepository = reelRepository;
        }

        public async Task<ReplyEventType> CreateReply(ReplyType reply)
        {
            // Mensajes de éxito y error
            string successMessage = "The reply was added successfully";
            string errorMessage = "There was an error while adding the reply";

            try
            {
                // Buscar el usuario que crea la respuesta por su ID.
                var user = await _userSQLDbRepository.FindUserById(reply.UserId);

                // Verificar si el usuario existe.
                if (user is null)
                {
                    // En caso de usuario no encontrado, registrar un error y devolver una respuesta apropiada.
                    _logger.LogError(errorMessage);
                    return ReplyEventType.CreateError(reply.UserId, HttpStatusCode.NotFound, errorMessage);
                }

                var isAdded = false;
                Guid mediaOwnerUserId = Guid.Empty;

                // Mapear los datos de respuesta a la entidad de respuesta y establecer la fecha actual.
                var replyToDb = MediaMapper.MapReplyTypeToReplyEntity(reply);
                replyToDb.Date = DateTime.UtcNow;

                // Realizar acciones basadas en el tipo de contenido (Post o Reel).
                switch (reply.ContentType)
                {
                    case ContentEnum.Post:
                        // Buscar el post al que se agrega la respuesta.
                        var post = await _postRepository.FindPostByIdAsync(reply.MediaId);
                        // Obtener el ID del dueño del contenido.
                        mediaOwnerUserId = post?.UserId ?? Guid.Empty;
                        // Intentar agregar la respuesta al post.
                        isAdded = post != null && await _postRepository.AddReplyAsync(replyToDb, reply.MediaId, reply.CommentId);
                        break;
                    case ContentEnum.Reel:
                        // Buscar el reel al que se agrega la respuesta.
                        var reel = await _reelRepository.FindReelById(reply.MediaId);
                        // Obtener el ID del dueño del contenido.
                        mediaOwnerUserId = reel?.UserId ?? Guid.Empty;
                        // Intentar agregar la respuesta al reel.
                        isAdded = reel != null && await _reelRepository.AddReplyAsync(replyToDb, reply.MediaId, reply.CommentId);
                        break;
                }

                // Intentar agregar la respuesta al reel.
                var logMessage = isAdded ? successMessage : errorMessage;
                _logger.LogInformation(logMessage);

                return ReplyEventType.CreateSuccess(
                    reply.UserId, 
                    reply.Text!, 
                    user.Username, 
                    reply.ReplyId!, 
                    mediaOwnerUserId,
                    logMessage);
            }
            catch (Exception ex)
            {
                // En caso de excepción, registrar un mensaje de error y devolver una respuesta de error.
                _logger.LogError($"An error occurred: {ex.Message}");
                return ReplyEventType.CreateError(reply.UserId,HttpStatusCode.InternalServerError, errorMessage);
            }
        }


        public async Task<ResponseType<string>> DeleteReply(ReplyType reply)
        {
            // Variables para controlar el éxito o error y mensajes asociados.
            bool isDeleted = false;
            string successMessage = "The reply was deleted successfully";
            string errorMessage = "There was an error while deleting the reply";

            // Buscar el usuario que está realizando la eliminación por su ID.
            var user = await _userSQLDbRepository.FindUserById(reply.UserId);

            // Verificar si el usuario existe.
            if (user is null)
            {
                // Si el usuario no se encuentra, devolver una respuesta de error.
                return ResponseType<string>.CreateErrorResponse(
                    System.Net.HttpStatusCode.NotFound,
                    errorMessage
                    );
            }

            var replyToDb = MediaMapper.MapReplyTypeToReplyEntity(reply);

            switch (reply.ContentType)
            {
                case ContentEnum.Post:
                    // Intentar eliminar la respuesta del post.
                    isDeleted = await _postRepository.DeleteReplyAsync(reply.MediaId, reply.CommentId, replyToDb);
                    break;
                case ContentEnum.Reel:
                    // Intentar eliminar la respuesta del reel.
                    isDeleted = await _reelRepository.DeleteReplyAsync(reply.MediaId, reply.CommentId, replyToDb);
                    break;
            }

            // Crear una respuesta en función de si la eliminación tuvo éxito o no.

            if (isDeleted)
            {
                return ResponseType<string>.CreateSuccessResponse(
                    null,
                    HttpStatusCode.NoContent,
                    successMessage);
            }

            return ResponseType<string>.CreateErrorResponse(
                System.Net.HttpStatusCode.InternalServerError,
                errorMessage
                );
        }
    }
}
