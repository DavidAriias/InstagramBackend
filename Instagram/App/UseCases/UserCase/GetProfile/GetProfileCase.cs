using Instagram.App.UseCases.UserCase.Types;
using Instagram.Domain.Repositories.Interfaces.Cache;
using Instagram.Domain.Repositories.Interfaces.Document.Post;
using Instagram.Domain.Repositories.Interfaces.Document.Reel;
using Instagram.Domain.Repositories.Interfaces.Document.Story;
using Instagram.Domain.Repositories.Interfaces.Graph.User;
using Instagram.Domain.Repositories.Interfaces.SQL.User;
using Instagram.Infraestructure.Mappers.User;
using Newtonsoft.Json;

namespace Instagram.App.UseCases.UserCase.GetProfile
{
    public class GetProfileCase : IGetProfileCase
    {
        private readonly IUserSQLDbRepository _userRepository;
        private readonly IRedisRepository _redisRepository;
        private readonly IPostMongoDbRepository _postRepository;
        private readonly IReelMongoDbRepository _reelRepository;
        private readonly IStoryMongoDbRepository _storyRepository;
        private readonly IUserNeo4jRepository _neo4jRepository;
        public GetProfileCase(
            IUserSQLDbRepository userRepository, 
            IRedisRepository redisRepository,
            IPostMongoDbRepository postRepository,
            IReelMongoDbRepository reelRepository,
            IStoryMongoDbRepository storyRepository,
            IUserNeo4jRepository neo4JRepository
            ) 
        {
            _userRepository = userRepository;
            _redisRepository = redisRepository;
            _postRepository = postRepository;
            _reelRepository = reelRepository;
            _storyRepository = storyRepository;
            _neo4jRepository = neo4JRepository;
        }
        public async Task<UserTypeOut?> GetProfile(Guid userId)
        {
            // Intenta obtener el perfil desde la caché.
            var cachedProfile = await TryGetCachedProfile(userId);

            if (cachedProfile != null)
            {
                return cachedProfile;
            }

            // Si no se encuentra en la caché, obtén los datos del usuario y su media desde las bases de datos.
            var userSql = await _userRepository.GetAllDataAboutUserById(userId);
            var postUser = await _postRepository.GetAllPostsByIdAsync(userId.ToString());
            var reelUser = await _reelRepository.GetAllReelsByIdAsync(userId.ToString());
            var storyUser = await _storyRepository.GetAllStoriesByIdAsync(userId.ToString());
            var followers = await _neo4jRepository.FollowersFromUser(userId.ToString());
            var following = await _neo4jRepository.UsersFollowedByOthers(userId.ToString());

            if (userSql == null)
            {
                return null;
            }

            // Mapea los datos del usuario a un objeto UserTypeOut.
            userSql.Posts = postUser;
            userSql.Reels = reelUser;
            userSql.Stories = storyUser;

            userSql.PostsNumber = storyUser?.Count() ?? 0;
            userSql.FollowersCount = followers.Count();
            userSql.FollowingCount = following.Count();

            var profile = UserMapper.MapUserEntityToUserTypeOut(userSql);

            // Almacena el perfil en la caché para futuras consultas.
            await CacheProfile(userId, profile);

            // Devuelve el perfil.
            return profile;
        }

        private async Task<UserTypeOut?> TryGetCachedProfile(Guid userId)
        {
            // Intenta obtener el perfil desde la caché utilizando la clave única del usuario.
            var cache = await _redisRepository.GetAsync($"profile:{userId}");

            // Si se encuentra en la caché, deserializa y devuelve el perfil.
            return cache != null ? JsonConvert.DeserializeObject<UserTypeOut?>(cache) : null;
        }

        private async Task CacheProfile(Guid userId, UserTypeOut profile)
        {
            // Almacena el perfil en la caché con una clave única durante 5 minutos.
            await _redisRepository.SetAsync($"profile:{userId}", JsonConvert.SerializeObject(profile), 
                TimeSpan.FromMinutes(5));
        }

    }
}
