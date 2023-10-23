using Instagram.App.UseCases.MediaCases.Types.Shared.Media;
using Instagram.App.UseCases.Types.Shared;
using Instagram.Domain.Enums;
using Instagram.Domain.Repositories.Interfaces.Document.Post;
using Instagram.Domain.Repositories.Interfaces.Document.Reel;
using Instagram.Domain.Repositories.Interfaces.Graph.Post;
using Instagram.Domain.Repositories.Interfaces.Graph.Reel;
using System.Net;

namespace Instagram.App.UseCases.MediaCases.TagsCase
{
    public class TagsCase : ITagsCase
    {
        private readonly IPostMongoDbRepository _postRepository;
        private readonly IPostNeo4jRepository _postNeo4JRepository;
        private readonly IReelMongoDbRepository _reelRepository;
        private readonly IReelNeo4jRepository _reelNeo4jRepository;
        public TagsCase(
            IPostMongoDbRepository postRepository,
            IPostNeo4jRepository postNeo4JRepository,
            IReelMongoDbRepository reelRepository,
            IReelNeo4jRepository reelNeo4JRepository
            )
        {
            _postRepository = postRepository;
            _postNeo4JRepository = postNeo4JRepository;
            _reelRepository = reelRepository;
            _reelNeo4jRepository = reelNeo4JRepository;
        }

        public async Task<ResponseType<IReadOnlyList<string>>> UpdateTagsAsync(TagsTypeIn tagsType)
        {
          
            // Variable para indicar si se realizó una edición.
            bool isEdit = false;

            // Actualiza las etiquetas dependiendo del tipo de contenido.
            switch (tagsType.ContentType)
            {
                case ContentEnum.Post:
                    // Actualiza las etiquetas en MongoDB para un post.
                    isEdit = await _postRepository.EditTagsAsync(tagsType.Tags, tagsType.MediaId);
                    // Actualiza las etiquetas en Neo4j.
                    await _postNeo4JRepository.EditTagsAsync(tagsType.UserId.ToString(), tagsType.MediaId, tagsType.Tags);
                    break;
                case ContentEnum.Reel:
                    isEdit = await _reelRepository.EditTagsAsync(tagsType.Tags, tagsType.MediaId);
                    await _reelNeo4jRepository.EditTagsAsync(tagsType.UserId.ToString(), tagsType.MediaId, tagsType.Tags);
                    break;
            }


            // Si se realizó la edición correctamente, devuelve una respuesta exitosa.
            if (isEdit)
            {
                return ResponseType<IReadOnlyList<string>>.CreateSuccessResponse(
                    tagsType.Tags,
                    HttpStatusCode.OK,
                    "Tags have been updated.");
            }

            // Si no se realizó la edición, devuelve una respuesta de error interno del servidor.
            return ResponseType<IReadOnlyList<string>>.CreateErrorResponse(
                System.Net.HttpStatusCode.InternalServerError,
                "Failed to update tags."
                );
        }

    }
}
