using Instagram.App.UseCases.MediaCases.Types.Shared.Media;
using Instagram.App.UseCases.MediaCases.Types.Shared.Media.Events;
using Instagram.App.UseCases.Types.Shared;
using Instagram.Domain.Entities.Shared.Media;
using Instagram.Domain.Enums;
using Instagram.Domain.Repositories.Interfaces.Document.Post;
using Instagram.Domain.Repositories.Interfaces.Document.Reel;
using Instagram.Domain.Repositories.Interfaces.SQL.User;

namespace Instagram.App.UseCases.MediaCases.CommentCase
{
    public class CommentCase : ICommentCase
    {
        private readonly IPostMongoDbRepository _postRepository;
        private readonly IReelMongoDbRepository _reelRepository;
        private readonly IUserSQLDbRepository _userSQLRepository;
        public CommentCase(
            IPostMongoDbRepository mongoRepository, 
            IUserSQLDbRepository userSQLDbRepository,
            IReelMongoDbRepository reelRepository
            ) 
        {
            _postRepository = mongoRepository;
            _userSQLRepository = userSQLDbRepository;
            _reelRepository = reelRepository;
        }

        public async Task<CommentEventType> CreateComment(CommentTypeIn commentType)
        {
            // Verificar si el usuario existe
            var user = await _userSQLRepository.FindUserById(commentType.UserId);
            Guid mediaOwnerUserId = new();

            if (user is null)
            {
                // El usuario no existe para realizar un comentario.
                return CommentEventType.CreateError("The user doesn't exist for making a comment.",
                    System.Net.HttpStatusCode.NotFound);
            }

            string? commentId = "";

            var comment = new CommentEntity
            {
                UserId = commentType.UserId,
                Date = DateTime.UtcNow,
                Text = commentType.Comment!,
            };

            switch (commentType.ContentType)
            {
                case ContentEnum.Post:
                    // Agregar un comentario al contenido de tipo "Post".
                    commentId = await _postRepository.AddCommentAsync(comment, commentType.MediaId);
                    var post = await _postRepository.FindPostByIdAsync(commentType.MediaId);
                    mediaOwnerUserId = post!.UserId;
                    break;
                case ContentEnum.Story:
                    // Agrega lógica para el tipo "Story" si es necesario
                    break;
                case ContentEnum.Reel:
                    // Agregar un comentario al contenido de tipo "Reel".
                    commentId = await _reelRepository.AddCommentAsync(comment, commentType.MediaId);
                    var reel = await _reelRepository.FindReelById(commentType.MediaId);
                    mediaOwnerUserId = reel!.UserId;
                    break;
            };

            if (commentId is not null)
            {
                var receiver = await _userSQLRepository.FindUserById(mediaOwnerUserId);
                var sender = await _userSQLRepository.FindUserById(commentType.UserId);

                return CommentEventType.CreateSuccess(
                    commentId,
                    sender!.Id,
                    receiver!.Id,
                    sender!.Username,
                    commentType.Comment!,
                    "Add comment succesfully."
                    );

            }

            return CommentEventType.CreateError("Failed to add the comment.", 
                System.Net.HttpStatusCode.InternalServerError);
        }

        public async Task<ResponseType<string>> DeleteComment(CommentTypeIn commentType)
        {
            // Verificar si el usuario existe
            var user = await _userSQLRepository.FindUserById(commentType.UserId);

            if (user is null)
            {
                return ResponseType<string>.CreateErrorResponse(
                    System.Net.HttpStatusCode.Conflict,
                    "The user doesn't exist for making a comment."
                    );
            }

            bool isCommentDeleted = false;

            switch (commentType.ContentType)
            {
                case ContentEnum.Post:
                    // Eliminar el comentario del contenido de tipo "Post".
                    isCommentDeleted = await _postRepository
                        .DeleteCommentAsync(commentType.UserId.ToString(),
                        commentType.MediaId, commentType.CommentId!);
                    break;
                case ContentEnum.Story:
                    // Agrega lógica para el tipo "Story" si es necesario
                    break;
                case ContentEnum.Reel:
                    // Eliminar el comentario del contenido de tipo "Reel".
                    isCommentDeleted = await _reelRepository
                        .DeleteCommentAsync(commentType.UserId.ToString(), commentType.MediaId,
                         commentType.CommentId!);
                    break;
            }

            var response = isCommentDeleted
                ? ResponseType<string>.CreateSuccessResponse(
                    null,
                    System.Net.HttpStatusCode.NoContent
                    ,"The comment was deleted successfully.")

                : ResponseType<string>.CreateErrorResponse(
                System.Net.HttpStatusCode.InternalServerError,
                "Failed to delete the comment."
                );

            return response;
        }



    }
}
