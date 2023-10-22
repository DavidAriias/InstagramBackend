using Instagram.App.UseCases.MediaCases.Types.Shared.Media;
using Instagram.App.UseCases.MediaCases.Types.Shared.Media.Events;
using Instagram.App.UseCases.Types.Shared;
using Instagram.Domain.Enums;
using Instagram.Domain.Repositories.Interfaces.Document.Post;
using Instagram.Domain.Repositories.Interfaces.Document.Reel;
using Instagram.Domain.Repositories.Interfaces.Graph.Post;
using Instagram.Domain.Repositories.Interfaces.Graph.Reel;
using Instagram.Domain.Repositories.Interfaces.SQL.User;

namespace Instagram.App.UseCases.MediaCases.LikeCase
{
    public class LikeCase : ILikeCase
    {
        private readonly IPostMongoDbRepository _postRepository;
        private readonly IPostNeo4jRepository _postNeo4jRepository;
        private readonly IReelNeo4jRepository _reelNeo4jRepository;
        private readonly IReelMongoDbRepository _reelRepository;
        private readonly IUserSQLDbRepository _userSQLRepository;
        public LikeCase(
            IPostMongoDbRepository postRepository, 
            IPostNeo4jRepository postNeo4jRepository,
            IReelMongoDbRepository reelRepository,
            IReelNeo4jRepository reelNeo4jRepository,
            IUserSQLDbRepository userSQLRepository
            ) 
        {
            _postNeo4jRepository = postNeo4jRepository;
            _postRepository = postRepository;
            _reelRepository = reelRepository;
            _reelNeo4jRepository = reelNeo4jRepository;
            _userSQLRepository = userSQLRepository;
        }

        public async Task<LikeEventType> AddLikeToMedia(LikeTypeIn likeType)
        {
            // Variable para controlar si el like se ha añadido con éxito.
            bool isAdded = false;

            // Variable para almacenar el ID del usuario propietario de los medios.
            Guid mediaOwnerUserId = new();

            // Determinar el tipo de contenido (Post, Story, Reel) y procesar el like en consecuencia.
            switch (likeType.ContentType)
            {
                case ContentEnum.Post:
                    // Agregar un like a una publicación y actualizar el gráfico Neo4j.
                    isAdded = await _postRepository.AddLikeAsync(likeType.MediaId.ToString());
                    var post = await _postRepository.FindPostByIdAsync(likeType.MediaId);
                    mediaOwnerUserId = post!.UserId;
                    await _postNeo4jRepository.AddLikeAsync(likeType.MediaId.ToString(), likeType.UserId.ToString());
                    break;

                case ContentEnum.Story:
                    // Manejar los likes de historias (por implementar).
                    // Agregar el código aquí para gestionar los likes de las historias.
                    break;

                case ContentEnum.Reel:
                    // Agregar un like a un reel y actualizar el gráfico Neo4j.
                    isAdded = await _reelRepository.AddLikeAsync(likeType.MediaId.ToString());
                    var reel = await _reelRepository.FindReelById(likeType.MediaId);
                    mediaOwnerUserId = reel!.UserId;
                    await _reelNeo4jRepository.AddLikeAsync(likeType.MediaId.ToString(), likeType.UserId.ToString());
                    break;
            }

            // Comprobar si el like se ha añadido con éxito.
            if (isAdded)
            {
                // Obtener información sobre el receptor y el remitente.
                var receiver = await _userSQLRepository.FindUserById(mediaOwnerUserId);
                var sender = await _userSQLRepository.FindUserById(likeType.UserId);

                // Devolver un evento con el mensaje de éxito y detalles del like.
                return new LikeEventType
                {
                    Message = "The like was added successfully",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    SenderId = likeType.UserId,
                    ReceiverId = receiver!.Id,
                    SenderUsername = sender!.Username
                };
            }

            // Si no se añadió el like con éxito, devolver un evento de error.
            return new LikeEventType
            {
                Message = "The like was added successfully",
                StatusCode = System.Net.HttpStatusCode.InternalServerError
            };
        }



        public async Task<ResponseType<string>> RemoveLikeToMedia(LikeTypeIn typeIn)
        {
            var isAdded = await _postRepository.DeleteLikeAsync(typeIn.MediaId.ToString());
            await _postNeo4jRepository.DeleteLikeAsync(typeIn.MediaId.ToString(), typeIn.UserId.ToString());

            var response = new ResponseType<string>();

            if (isAdded)
            {
                response.Message = "The like was removed successfully";
                response.StatusCode = System.Net.HttpStatusCode.OK;
            }
            else
            {
                response.Message = "There was an error while removing the like";
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            }

            return response;
        }
    }
}
