using Instagram.App.UseCases.MediaCases.Types.Stories;
using Instagram.App.UseCases.Types.Feed;
using Instagram.App.UseCases.Types.Shared;
using Instagram.config.helpers;
using Instagram.Domain.Entities.User;
using Instagram.Domain.Enums;
using Instagram.Domain.Repositories.Interfaces.Blob;
using Instagram.Domain.Repositories.Interfaces.Cache;
using Instagram.Domain.Repositories.Interfaces.Document.Story;
using Instagram.Domain.Repositories.Interfaces.Graph.User;
using Instagram.Domain.Repositories.Interfaces.SQL.User;
using Instagram.Infraestructure.Mappers.Feed;
using Instagram.Infraestructure.Mappers.Story;
using Newtonsoft.Json;

namespace Instagram.App.UseCases.MediaCases.StoryCase
{
    public class StoryCase : IStoryCase
    {
        private readonly IStoryMongoDbRepository _storyRepository;
        private readonly IImageBlobService _imageBlobService;
        private readonly IVideoBlobService _videoBlobService;
        private readonly ILogger<StoryCase> _logger;
        private readonly IUserNeo4jRepository _userNeo4JRepository;
        private readonly IUserSQLDbRepository _userSQLRepository;
        private readonly IRedisRepository _redisRepository;
        public StoryCase(
            IStoryMongoDbRepository storyRepository, 
            ILogger<StoryCase> logger,
            IImageBlobService imageBlobService,
            IVideoBlobService videoBlobService,
            IUserSQLDbRepository userSQLRepository,
            IUserNeo4jRepository userNeo4JRepository,
            IRedisRepository redisRepository
            ) 
        {
            _storyRepository = storyRepository;
            _logger = logger;
            _imageBlobService = imageBlobService;
            _videoBlobService = videoBlobService;
            _userSQLRepository = userSQLRepository;
            _userNeo4JRepository = userNeo4JRepository;
            _redisRepository = redisRepository;
        }

        public async Task<StoryEventType> CreateStory(StoryTypeIn storyType)
        {
            // Inicializamos mediaUrl como una cadena vacía.
            string? mediaUrl = "";

            // Utilizamos un switch para determinar el tipo de medios y cargar el archivo correspondiente.
            switch (storyType.Media.DataType)
            {
                case MediaEnum.Image:
                    //Utilizamos el servicio de imagenes para subirlas al blob
                    mediaUrl = await _imageBlobService.UploadStoryAsync(storyType.Media.Media, storyType.UserId);
                    break;
                case MediaEnum.Video:
                    //Utilizamos el servicio de videos para subirlas al blob
                    mediaUrl = await _videoBlobService.UploadStoryAsync(storyType.Media.Media, storyType.UserId);
                    break;
            }

            // Mapeamos los datos de StoryTypeIn a una entidad de historia.
            var storyEntity = StoryMapper.MapReelTypeInToReelEntity(storyType);

            // Establecemos la fecha de publicación actual y de expiracion.
            var date = DateTime.UtcNow;
            storyEntity.PostDate = date;

            // Creamos la historia en el repositorio y obtenemos su ID.
            var storyId = await _storyRepository.CreateStoryAsync(storyEntity);

            // Verificamos si la URL del medio o el ID de la historia son nulos.
            if (mediaUrl is null || storyId is null)
            {
                // Registramos el error.
                _logger.LogError("The story couldn't be uploaded.");

                // Si hay un problema, devolvemos una respuesta de error.
                return StoryEventType.CreateErrorResponse();
               
            }

            // Registramos la carga exitosa.
            _logger.LogInformation("The media has been uploaded successfully.");

            // Si todo va bien, devolvemos una respuesta exitosa.
            return StoryEventType
                .CreateSuccessResponse(
                storyType.UserId, 
                storyType.Duration, 
                storyType.Location, 
                mediaUrl, 
                date,
                storyType.Music);
        }

        public async Task<ResponseType<string>> DeleteStory(string storyId, Guid userId)
        {
            // Intentamos encontrar la historia por su ID en el repositorio.
            var story = await _storyRepository.FindStoryById(storyId);

            if (story is null)
            {
                // Si no encontramos la historia, devolvemos un error.
                return ResponseType<string>.CreateErrorResponse("Story not found", System.Net.HttpStatusCode.NotFound);
            }

            // Determinamos si el medio es una imagen o un video.
            bool isImage = MediaHelper.IsImage(story.MediaUrl);

            // Eliminamos el medio del almacenamiento Blob correspondiente (imagen o video).
            bool isDeletedStoryInBlob = isImage
                ? await _imageBlobService.DeleteImageAsync(story.MediaUrl)
                : await _videoBlobService.DeleteVideoAsync(story.MediaUrl);

            if (!isDeletedStoryInBlob)
            {
                // Si la eliminación en el almacenamiento Blob falla, devolvemos un error.
                return ResponseType<string>.CreateErrorResponse("Failed to delete media from storage", System.Net.HttpStatusCode.NotFound);
            }

            // Intentamos eliminar la historia de MongoDB.
            bool isDeletedStoryInMongo = await _storyRepository.DeleteStoryAsync(storyId);

            if (!isDeletedStoryInMongo || !isDeletedStoryInBlob)
            {
                // Si la eliminación en MongoDB falla, devolvemos un error.
                return ResponseType<string>.CreateErrorResponse("Failed to delete story", System.Net.HttpStatusCode.InternalServerError);
            }

            // Si todas las eliminaciones fueron exitosas, devolvemos una respuesta exitosa.
            return ResponseType<string>.CreateSuccessResponse("The story has been deleted");
        }

        public async Task<IReadOnlyList<FeedType<StoryTypeOut>>> GetStoriesByUserId(Guid userId)
        {
            // Crear una lista para almacenar los feeds de stories.
            var feedStories = new List<FeedType<StoryTypeOut>>();

            try
            {
                // Intenta obtener los reels desde la caché.
                var cachedStories = await TryGetCachedStories(userId);

                if (cachedStories is not null) return cachedStories;

                // Obtener la lista de usuarios seguidos por el usuario actual desde la base de datos Neo4J.
                var followedUsers = await _userNeo4JRepository.UsersFollowedByOthers(userId.ToString());

                // Recorrer la lista de usuarios seguidos.
                foreach (var followedUser in followedUsers)
                {
                    // Obtener las últimas stories del usuario seguido en los últimos 3 días desde la base de datos MongoDB.
                    var stories = await _storyRepository.GetAllStoriesByIdAsync(followedUser);

                    // Verificar si hay stories y si el usuario existe.
                    if (stories is not null && stories.Any())
                    {
                        // Ordenar las stories por fecha de publicación en orden descendente (las más recientes primero).
                        var sortedStories = stories.OrderByDescending(story => story.PostDate).ToList();

                        // Obtener detalles del usuario seguido desde la base de datos SQL.
                        var user = await GetUserDetails(Guid.Parse(followedUser));

                        // Crear un feed de stories y agregarlo a la lista.
                        var feed = FeedMapper.MapFeedStory(user!, sortedStories);
                        feedStories.Add(feed);
                    }
                }
            }
            catch (Exception ex)
            {
                // En caso de error, registrar el error en el registro.
                _logger.LogError(ex, "Error while fetching stories for user with ID: {UserId}", userId);
            }

            if (feedStories is not null) await CacheStories(userId, feedStories);

            // Devolver la lista de feeds de stories.
            return feedStories!;
        }

        private async Task<UserEntity?> GetUserDetails(Guid userId)
        {
            return await _userSQLRepository.FindUserById(userId);
        }

        private async Task<IReadOnlyList<FeedType<StoryTypeOut>>?> TryGetCachedStories(Guid userId)
        {
            // Intenta obtener el perfil desde la caché utilizando la clave única del usuario.
            var cache = await _redisRepository.GetAsync($"stories:{userId}");

            // Si se encuentra en la caché, deserializa y devuelve el perfil.
            return cache != null ? JsonConvert
                .DeserializeObject<IReadOnlyList<FeedType<StoryTypeOut>>>(cache) : null;
        }

        private async Task CacheStories(Guid userId, IReadOnlyList<FeedType<StoryTypeOut>> stories)
        {
            // Almacena el perfil en la caché con una clave única durante 5 minutos.
            await _redisRepository.SetAsync($"stories:{userId}",
                JsonConvert.SerializeObject(stories),
                TimeSpan.FromMinutes(5));
        }
    }
}
