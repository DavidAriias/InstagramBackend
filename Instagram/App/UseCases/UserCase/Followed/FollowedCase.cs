using Instagram.App.UseCases.UserCase.Types;
using Instagram.App.UseCases.UserCase.Types.Events;
using Instagram.Domain.Repositories.Interfaces.Cache;
using Instagram.Domain.Repositories.Interfaces.Graph.User;
using Instagram.Domain.Repositories.Interfaces.SQL.User;
using Newtonsoft.Json;
using SpotifyAPI.Web;

namespace Instagram.App.UseCases.UserCase.Followed
{
    public class FollowedCase : IFollowedCase
    {
        private readonly IUserNeo4jRepository _neo4JRepository;
        private readonly IUserSQLDbRepository _userSQLRepository;
        private readonly ILogger<FollowedCase> _logger;
        private readonly IRedisRepository _redisRepository;
        public FollowedCase(
            IUserNeo4jRepository neo4JRepository,
            IUserSQLDbRepository userSQLRepository,
            ILogger<FollowedCase> logger,
            IRedisRepository redisRepository
            )
        {
            _neo4JRepository = neo4JRepository;
            _userSQLRepository = userSQLRepository;
            _logger = logger;
            _redisRepository = redisRepository;
        }

        public async Task<FollowedEventType> FollowToUser(Guid followerId, Guid followedId)
        {
            try
            {
                // Realizar la operación de seguimiento en la base de datos Neo4J.
                await _neo4JRepository.FollowToUser(followerId.ToString(), followedId.ToString());

                // Obtener los datos del usuario seguidor desde la base de datos SQL.
                var follower = await _userSQLRepository.FindUserById(followerId);

                // Crear y devolver un objeto FollowedEventType con la información del seguimiento.
                return FollowedEventType.Create(followedId, followerId, follower!.Username, follower.Imageprofile);
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir durante la operación de seguimiento y registrar un error.
                _logger.LogError(ex, "Error while following a user.");

                // En caso de error, devolver un objeto FollowedEventType con valores predeterminados.
                return new FollowedEventType();
            }
        }

        public async Task<IEnumerable<FollowerType>?> GetUsersFollowedByOthers(Guid userId)
        {
            try
            {
                // Intenta obtener los seguidos desde la caché.
                var cachedFollowed = await TryGetCachedFollowed(userId);

                if (cachedFollowed is not null) return cachedFollowed;
                // Lista para almacenar los seguidores.
                var followerList = new List<FollowerType>();

                // Obtener los IDs de los usuarios seguidos por el usuario actual desde Neo4J.
                var userFollowedIds = await _neo4JRepository.UsersFollowedByOthers(userId.ToString());

                // Recorrer la lista de IDs de usuarios seguidos.
                foreach (var followedUserId in userFollowedIds)
                {
                    // Obtener los datos del usuario seguido desde SQL.
                    var userData = await _userSQLRepository.FindUserById(Guid.Parse(followedUserId));

                    // Verificar si se encontraron datos del usuario seguido.
                    if (userData != null)
                    {
                        // Crear un objeto FollowerType para el seguidor.
                        followerList.Add(FollowerType
                            .Create
                            (Guid.Parse(followedUserId),
                            userData.Imageprofile,
                            userData.Username));
                    }
                }

                if (cachedFollowed != null) await CacheFollowed(userId, followerList);
                // Devolver la lista de seguidores.
                return followerList;
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir durante la obtención de seguidores y registrar un error.
                _logger.LogError(ex, "Error while fetching users followed by others.");
            }

            // En caso de error, devolver null.
            return null;
        }

        private async Task<IEnumerable<FollowerType>?> TryGetCachedFollowed(Guid userId)
        {
            // Intenta obtener el perfil desde la caché utilizando la clave única del usuario.
            var cache = await _redisRepository.GetAsync($"followed:{userId}");

            // Si se encuentra en la caché, deserializa y devuelve los seguidos.
            return cache != null ? JsonConvert
                .DeserializeObject<IEnumerable<FollowerType>?>(cache) : null;
        }

        private async Task CacheFollowed(Guid userId, IEnumerable<FollowerType> follower)
        {
            // Almacena el perfil en la caché con una clave única durante 5 minutos.
            await _redisRepository.SetAsync($"followed:{userId}", JsonConvert.SerializeObject(follower),
                TimeSpan.FromMinutes(5));
        }

    }
}
