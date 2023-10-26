using Instagram.App.UseCases.MediaCases.Types.Posts;
using Instagram.App.UseCases.MediaCases.Types.Shared.Media;
using Instagram.App.UseCases.Types.Shared;
using Instagram.config.helpers;
using Instagram.Domain.Entities.Media;
using Instagram.Domain.Entities.User;
using Instagram.Domain.Enums;
using Instagram.Domain.Repositories.Interfaces.Blob;
using Instagram.Domain.Repositories.Interfaces.Cache;
using Instagram.Domain.Repositories.Interfaces.Document.Post;
using Instagram.Domain.Repositories.Interfaces.Graph.Post;
using Instagram.Domain.Repositories.Interfaces.Graph.User;
using Instagram.Domain.Repositories.Interfaces.SQL.User;
using Instagram.Infraestructure.Mappers;
using Newtonsoft.Json;
using System.Net;

namespace Instagram.App.UseCases.MediaCases.PostCase
{
    public partial class PostCase : IPostCase
    {
        private readonly IPostMongoDbRepository _mongoRepository;
        private readonly IPostNeo4jRepository _neo4JRepository;
        private readonly IImageBlobService _imageBlobService;
        private readonly IVideoBlobService _videoBlobService;
        private readonly ILogger<PostCase> _logger;
        private readonly IRedisRepository _redisRepository;
        private readonly IUserNeo4jRepository _userNeo4JRepository;
        private readonly IUserSQLDbRepository _userSQLDbRepository;
        public PostCase(
            IPostMongoDbRepository mongoRepository,
            IPostNeo4jRepository mediaNeo4JRepository,
            IImageBlobService imageBlobService,
            IVideoBlobService videoBlobService,
            ILogger<PostCase> logger,
            IRedisRepository redisRepository,
            IUserNeo4jRepository userNeo4JRepository,
            IUserSQLDbRepository userSQLDbRepository
            ) 
        {
            _mongoRepository = mongoRepository;
            _neo4JRepository = mediaNeo4JRepository;
            _imageBlobService = imageBlobService;
            _videoBlobService = videoBlobService;
            _logger = logger;
            _redisRepository = redisRepository;
            _userNeo4JRepository = userNeo4JRepository;
            _userSQLDbRepository = userSQLDbRepository;
        }

        public async Task<PostEventType> CreatePostAsync(PostTypeIn postIn)
        {
            try
            {
                // Sube el contenido multimedia y obtén las URL de los medios.
                var mediaUrls = await UploadMedia(postIn.Media, postIn.UserId);

                if (mediaUrls.Count == 0)
                {
                    // Si no se obtienen las URL de los medios, devuelve una respuesta de error.
                    return PostEventType.CreateError(HttpStatusCode.InternalServerError, 
                        "Post can't be uploaded now, try later");
                }

                // Mapea los datos de entrada a una entidad de publicación.
                var postEntity = PostMapper.MapPostTypeInToPostEntity(postIn, mediaUrls);

                // Establece la fecha de publicación actual.
                var date = DateTime.UtcNow;
                postEntity.DatePublication = date;

                // Crea la entidad de publicación en la base de datos.
                var postId = await CreatePostEntities(postEntity);

                // Devuelve una respuesta exitosa con las URL de los medios.
                return PostEventType.Create(
                    date, 
                    mediaUrls, 
                    postIn.Caption, 
                    postIn.Music, 
                    postIn.Tags, 
                    postIn.Location,
                    postIn.Mentions,
                    postIn.UserId);
            }
            catch (Exception ex)
            {
                // Registra cualquier error que ocurra durante la carga o creación de medios.
                _logger.LogError($"Error uploading or creating media: {ex.Message}");
                return PostEventType.CreateError(HttpStatusCode.InternalServerError, 
                    "Failed to create the post.");
            }
        }


        private async Task<List<string>> UploadMedia(List<MediaType> mediaItems, Guid userId)
        {
            var mediaUrls = new List<string>();

            var uploadTasks = mediaItems.Select(async item =>
            {
                if (item.DataType == MediaEnum.Image)
                {
                    return await _imageBlobService.UploadPostImageAsync(item.Media, userId);
                }
                else if (item.DataType == MediaEnum.Video)
                {
                    return await _videoBlobService.UploadPostVideoAsync(item.Media, userId);
                }
                else
                {
                    return null;
                }
            }).ToArray(); // Convert the sequence into an array of tasks

            // Wait for all upload tasks to complete.
            var results = await Task.WhenAll(uploadTasks);

            foreach (var url in results)
            {
                if (url == null)
                {
                    _logger.LogError("Media upload was aborted due to an error in media upload.");
                    return new List<string>(); // Return an empty list in case of failure
                }
                mediaUrls.Add(url);
            }

            if (mediaUrls.Count == 0)
            {
                _logger.LogError("Media upload was aborted due to an error in media upload.");
            }
            else
            {
                _logger.LogInformation("All media has been successfully uploaded to Blob storage.");
            }

            return mediaUrls;
        }


        private async Task<string?> CreatePostEntities(PostEntity postEntity)
        {
            var postId = await _mongoRepository.CreatePostAsync(postEntity);
            await _neo4JRepository.CreatePostNodeAsync(postEntity);

            if (postId is not null) _logger.LogInformation($"Post created successfully with ID: {postId}");
            else
            _logger.LogError("Post can't created successfully");

            return postId;
        }
        public async Task<ResponseType<string>> DeletePostAsync(string postId, Guid userId)
        {
            // Variable para rastrear si se eliminaron los medios asociados al post.
            bool isDeletedMedia = false;

            // Busca el post en MongoDB.
            var post = await _mongoRepository.FindPostByIdAsync(postId);

            // Verifica si se encontró el post en MongoDB.
            if (post is not null)
            {
                // Elimina las imágenes o videos asociados al post.
                foreach (var url in post.Images)
                {
                    bool isImage = MediaHelper.IsImage(url);

                    // Borra la imagen o el video en función de la URL.
                    if (isImage)
                    {
                        await _imageBlobService.DeleteImageAsync(url);
                    }
                    else
                    {
                        await _videoBlobService.DeleteVideoAsync(url);
                    }
                }

                // Marcamos que los medios han sido eliminados.
                isDeletedMedia = true;
            }

            // Elimina el post de MongoDB y de Neo4j.
            bool isDeletedFromMongo = await _mongoRepository.DeletePostAsync(postId);
            await _neo4JRepository.DeletePostNodeAsync(userId.ToString(), postId);

            // Comprueba si el post se eliminó correctamente en ambos almacenes de datos.
            if (isDeletedFromMongo && isDeletedMedia)
            {
                return ResponseType<string>.CreateSuccessResponse(
                    null, 
                    HttpStatusCode.NoContent,
                    "Post deleted successfully.");
            }
            else
            {
                return ResponseType<string>.CreateErrorResponse(
                    System.Net.HttpStatusCode.InternalServerError,
                    "Failed to delete the post."
                    );
            }
        }

        public async Task<IReadOnlyList<PostTypeOut>> GetFeedPostByUserId(Guid userId)
        {
            // Crear una lista para almacenar los feeds de publicaciones.
            var feedPosts = new List<PostTypeOut>();

            try
            {

                // Intenta obtener los psots desde la caché.
                var cachedPosts = await TryGetCachedPosts(userId);

                if (cachedPosts is not null) return cachedPosts;

                // Obtener la lista de usuarios seguidos por el usuario actual desde la base de datos Neo4J.
                var followedUsers = await _userNeo4JRepository.UsersFollowedByOthers(userId.ToString());

                // Recorrer la lista de usuarios seguidos.
                foreach (var followedUser in followedUsers)
                {
                    // Obtener las últimas publicaciones del usuario seguido en los últimos 3 días desde la base de datos MongoDB.
                    var lastPosts = await _mongoRepository.GetLastPostsIn3DaysAsync(followedUser);

                    // Verificar si hay publicaciones y si el usuario existe.
                    if (lastPosts is not null && lastPosts.Any())
                    {
                        // Obtener detalles del usuario seguido desde la base de datos SQL.
                        var user = await GetUserDetails(Guid.Parse(followedUser));

                        // Crear un feed de publicaciones y agregarlo a la lista.
                        feedPosts = lastPosts.Select(post =>
                        {
                            post.ImageProfile = user!.Imageprofile;
                            post.Username = user!.Username;
                            return PostMapper.MapPostEntityToPostTypeOut(post);
                        }).ToList();
                       
                    }
                }
            }
            catch (Exception ex)
            {
                // En caso de error, registrar el error en el registro.
                _logger.LogError(ex, "Error while fetching posts for user with ID: {UserId}", userId);
            }

            if (feedPosts is not null) await CachedPosts(userId, feedPosts);
            // Devolver la lista de feeds de publicaciones.
            return feedPosts!;
        }

        private async Task<UserEntity?> GetUserDetails(Guid userId)
        {
            return await _userSQLDbRepository.FindUserById(userId);
        }
        private async Task<IReadOnlyList<PostTypeOut>?> TryGetCachedPosts(Guid userId)
        {
            // Intenta obtener el perfil desde la caché utilizando la clave única del usuario.
            var cache = await _redisRepository.GetAsync($"posts:{userId}");

            // Si se encuentra en la caché, deserializa y devuelve el perfil.
            return cache != null ? JsonConvert
                .DeserializeObject<IReadOnlyList<PostTypeOut>>(cache) : null;
        }

        private async Task CachedPosts(Guid userId, IReadOnlyList<PostTypeOut> posts)
        {
            // Almacena el perfil en la caché con una clave única durante 5 minutos.
            await _redisRepository.SetAsync($"posts:{userId}",
                JsonConvert.SerializeObject(posts),
                TimeSpan.FromMinutes(5));
        }
    }
}
