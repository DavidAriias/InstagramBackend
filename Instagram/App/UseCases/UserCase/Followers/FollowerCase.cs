using Instagram.App.UseCases.Types.Shared;
using Instagram.App.UseCases.UserCase.Types;
using Instagram.Domain.Repositories.Interfaces.Cache;
using Instagram.Domain.Repositories.Interfaces.Graph.User;
using Instagram.Domain.Repositories.Interfaces.SQL.User;
using Newtonsoft.Json;

namespace Instagram.App.UseCases.UserCase.Followers
{
    public class FollowerCase : IFollowerCase
    {
        private readonly IUserNeo4jRepository _neo4JRepository;
        private readonly IUserSQLDbRepository _userSQLRepository;
        private readonly IRedisRepository _redisRepository;
        private readonly ILogger<FollowerCase> _logger; 
        public FollowerCase(
            IUserNeo4jRepository neo4JRepository,
            IUserSQLDbRepository userSQLRepository,
            ILogger<FollowerCase> logger,
            IRedisRepository redisRepository)
        {
            _neo4JRepository = neo4JRepository;
            _userSQLRepository = userSQLRepository;
            _logger = logger;
            _redisRepository = redisRepository;
        }

        public async Task<IEnumerable<FollowerType>?> GetFollowersFromUser(Guid userId)
        {
            try
            {
                // Intenta obtener los seguidores desde la caché.
                var cachedFollowers = await TryGetCachedFollowers(userId);

                if (cachedFollowers is not null) return cachedFollowers;

                // Obtener los IDs de los seguidores del usuario desde Neo4J.
                var userFollowedIds = await _neo4JRepository.FollowersFromUser(userId.ToString());

                // Crear una lista para almacenar los seguidores.
                var followers = new List<FollowerType>();

                // Recorrer la lista de IDs de seguidores y crear objetos FollowerType.
                foreach (var followedUserId in userFollowedIds)
                {
                    var userData = await _userSQLRepository.FindUserById(Guid.Parse(followedUserId));

                    if (userData != null)
                    {
                        followers.Add(FollowerType
                            .Create
                            (Guid.Parse(followedUserId),
                            userData.Imageprofile,
                            userData.Username));
                    }
                }

                if (cachedFollowers != null) await CacheFollowed(userId, followers);
                // Devolver la lista de seguidores.
                return followers;
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir durante la obtención de seguidores y registrar un error.
                _logger.LogError(ex, "Error while fetching followers from the user.");
            }

            // En caso de error, devolver null.
            return null;
        }

        private async Task<IEnumerable<FollowerType>?> TryGetCachedFollowers(Guid userId)
        {
            // Intenta obtener el perfil desde la caché utilizando la clave única del usuario.
            var cache = await _redisRepository.GetAsync($"followers:{userId}");

            // Si se encuentra en la caché, deserializa y devuelve los seguidos.
            return cache != null ? JsonConvert
                .DeserializeObject<IEnumerable<FollowerType>?>(cache) : null;
        }

        private async Task CacheFollowed(Guid userId, IEnumerable<FollowerType> follower)
        {
            // Almacena el perfil en la caché con una clave única durante 5 minutos.
            await _redisRepository.SetAsync($"followers:{userId}", JsonConvert.SerializeObject(follower),
                TimeSpan.FromMinutes(5));
        }

        public async Task<bool> IsUserFollowToOther(Guid followerId, Guid userId)
        {
            return await _neo4JRepository
                .IsUserFollowToOther(followerId.ToString(), userId.ToString());
        }

        public async Task<ResponseType<Guid>> UnfollowUser(Guid followerId, Guid userId)
        {
            await _neo4JRepository.UnfollowUser(followerId.ToString(), userId.ToString());

            return new ResponseType<Guid>
            {
                Value = userId,
                Message = $"The user with id {followerId} unfollows to user with id {userId}",
                StatusCode = System.Net.HttpStatusCode.OK,
            };
        }
    }
}
