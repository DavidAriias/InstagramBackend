using Instagram.App.UseCases.MediaCases.Types.Reels;
using Instagram.App.UseCases.Types.Feed;
using Instagram.App.UseCases.Types.Shared;
using Instagram.Domain.Entities.User;
using Instagram.Domain.Repositories.Interfaces.Blob;
using Instagram.Domain.Repositories.Interfaces.Cache;
using Instagram.Domain.Repositories.Interfaces.Document.Reel;
using Instagram.Domain.Repositories.Interfaces.Graph.Reel;
using Instagram.Domain.Repositories.Interfaces.Graph.User;
using Instagram.Domain.Repositories.Interfaces.SQL.User;
using Instagram.Infraestructure.Mappers.Reel;
using Newtonsoft.Json;
using System.Net;

namespace Instagram.App.UseCases.MediaCases.ReelCase
{
    public class ReelCase : IReelCase
    {
        private readonly IReelMongoDbRepository _reelRepository;
        private readonly ILogger<ReelCase> _logger;
        private readonly IVideoBlobService _videoBlobService;
        private readonly IReelNeo4jRepository _mediaNeo4JRepository;
        private readonly IUserNeo4jRepository _userNeo4JRepository;
        private readonly IUserSQLDbRepository _userSQLRepository;
        private readonly IRedisRepository _redisRepository;
        public ReelCase(
            IReelMongoDbRepository reelRepository, 
            ILogger<ReelCase> logger,
            IVideoBlobService videoBlobService,
            IReelNeo4jRepository mediaNeo4JRepository,
            IUserNeo4jRepository userNeo4jRepository,
            IUserSQLDbRepository userSQLRepository,
            IRedisRepository redisRepository
            ) 
        {
            _reelRepository = reelRepository;
            _logger = logger;
            _videoBlobService = videoBlobService;
            _mediaNeo4JRepository = mediaNeo4JRepository;
            _userSQLRepository = userSQLRepository;
            _userNeo4JRepository = userNeo4jRepository;
            _redisRepository = redisRepository;
        }

        public async Task<ReelEventType> CreateReelAsync(ReelTypeIn reelIn)
        {
            try
            {
                // Subir el video a través del servicio de almacenamiento de blobs
                var url = await _videoBlobService.UploadReelAsync(reelIn.File, reelIn.UserId);

                if (url is null)
                {
                    // Si la carga del video falla, devuelve una respuesta de error.
                    return ReelEventType.CreateError(HttpStatusCode.InternalServerError, reelIn.UserId, "Reel can't be uploaded, please try again later.");
                }

                // Mapear los datos del ReelTypeIn a una entidad Reel
                var reelToDb = ReelMapper.MapReelTypeInToReelEntity(reelIn);
                var date = DateTime.UtcNow;
                reelToDb.DatePublication = date;

                // Crear el Reel en la base de datos principal
                var reelId = await _reelRepository.CreateReelAsync(reelToDb);

                // Crear un nodo en Neo4j
                var isSavedInNeo = await _mediaNeo4JRepository.CreateReelNodeAsync(reelToDb);

                if (reelId is not null && isSavedInNeo)
                {
                    // Si la creación del Reel en la base de datos principal y en Neo4j es exitosa,
                    // devuelve una respuesta exitosa con la URL del video.
                    return ReelEventType.CreateSuccess(
                         "Reel has been uploaded successfully",
                        reelIn.Caption, 
                        date,reelIn.Duration, 
                        reelIn.Location, 
                        reelIn.Mentions,
                        reelIn.Music,
                        reelIn.Tags, 
                        url,
                        reelIn.UserId
                        );
                }
            }
            catch (Exception ex)
            {
                // Captura cualquier excepción inesperada y registra el error.
                _logger.LogError($"Error creating reel: {ex.Message}");
            }

            // Devuelve una respuesta de error en caso de excepción.
            return ReelEventType.CreateError(
                HttpStatusCode.InternalServerError, 
                reelIn.UserId, 
                "An error occurred while creating the reel. Please try again later.");
        }



        public async Task<ResponseType<string>> DeleteReelAsync(string reelId, Guid userId)
        {
            // Elimina el post de MongoDB
            var reel = await _reelRepository.FindReelById(reelId);
            bool isDeletedVideo = false;

            if (reel is not null)
            {
                isDeletedVideo = await _videoBlobService.DeleteVideoAsync(reel.Url);
            }
            bool isDeletedFromMongo = await _reelRepository.DeleteReelAsync(reelId);

            // Elimina el post de Neo4j
            await _mediaNeo4JRepository.DeleteReelNodeAsync(userId.ToString(), reelId);

            // Comprueba si el post se eliminó correctamente en ambos almacenes de datos
            if (isDeletedFromMongo && isDeletedVideo)
            {
                return ResponseType<string>.CreateSuccessResponse(
                    null,
                    HttpStatusCode.NoContent,
                    "Post deleted successfully.");
            }
            else
            {
                return ResponseType<string>.CreateErrorResponse(
                    HttpStatusCode.InternalServerError,
                    "Failed to delete the post."
                    );
            }
        }

        public async Task<IReadOnlyList<ReelTypeOut>> GetFeedReelByUserId(Guid userId)
        {
            // Crear una lista para almacenar los feeds de reels.
            var feedReels = new List<ReelTypeOut>();

            try
            {
                // Intenta obtener los reels desde la caché.
                var cachedReels = await TryGetCachedReels(userId);

                if (cachedReels is not null) return cachedReels;

                // Obtener la lista de usuarios seguidos por el usuario actual desde la base de datos Neo4J.
                var followedUsers = await _userNeo4JRepository.UsersFollowedByOthers(userId.ToString());

                // Recorrer la lista de usuarios seguidos.
                foreach (var followedUser in followedUsers)
                {
                    // Obtener los últimos reels del usuario seguido en los últimos 3 días.
                    var lastReels = await _reelRepository.GetLastReelsIn3Days(followedUser);

                    // Verificar si hay reels y si el usuario existe.
                    if (lastReels is not null && lastReels.Any())
                    {
                        // Obtener detalles del usuario seguido desde la base de datos SQL.
                        var user = await GetUserDetails(Guid.Parse(followedUser));

                        // Crear un feed de reels y agregarlo a la lista.
                        feedReels = lastReels.Select(reel =>
                        {
                            reel.Username = user!.Username;
                            reel.ImageProfile = user!.Imageprofile;
                            return ReelMapper.MapReelEntityToReelTypeOut(reel);
                        }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                // En caso de error, registrar el error en el registro.
                _logger.LogError(ex, "Error while fetching reels for user with ID: {UserId}", userId);
            }

            if (feedReels is not null) await CacheReels(userId, feedReels);
            // Devolver la lista de feeds de reels.
            return feedReels!;
        }
        private async Task<UserEntity?> GetUserDetails(Guid userId)
        {
            return await _userSQLRepository.FindUserById(userId);
        }

        private async Task<IReadOnlyList<ReelTypeOut>?> TryGetCachedReels(Guid userId)
        {
            // Intenta obtener el perfil desde la caché utilizando la clave única del usuario.
            var cache = await _redisRepository.GetAsync($"reels:{userId}");

            // Si se encuentra en la caché, deserializa y devuelve el perfil.
            return cache != null ? JsonConvert
                .DeserializeObject<IReadOnlyList<ReelTypeOut>>(cache) : null;
        }

        private async Task CacheReels(Guid userId, IReadOnlyList<ReelTypeOut> reels)
        {
            // Almacena el perfil en la caché con una clave única durante 5 minutos.
            await _redisRepository.SetAsync($"reels:{userId}", 
                JsonConvert.SerializeObject(reels),
                TimeSpan.FromMinutes(5));
        }
    }
}
