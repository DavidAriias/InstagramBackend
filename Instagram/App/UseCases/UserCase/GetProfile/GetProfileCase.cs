using Instagram.App.UseCases.UserCase.Types;
using Instagram.Domain.Repositories.Interfaces.Cache;
using Instagram.Domain.Repositories.Interfaces.Document.Post;
using Instagram.Domain.Repositories.Interfaces.Document.Reel;
using Instagram.Domain.Repositories.Interfaces.Document.Story;
using Instagram.Domain.Repositories.Interfaces.SQL.User;
using Instagram.Infraestructure.Mappers;
using Instagram.Infraestructure.Mappers.Reel;
using Instagram.Infraestructure.Mappers.Story;
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
        public GetProfileCase(
            IUserSQLDbRepository userRepository, 
            IRedisRepository redisRepository,
            IPostMongoDbRepository postRepository,
            IReelMongoDbRepository reelRepository,
            IStoryMongoDbRepository storyRepository
            ) 
        {
            _userRepository = userRepository;
            _redisRepository = redisRepository;
            _postRepository = postRepository;
            _reelRepository = reelRepository;
            _storyRepository = storyRepository;
        }
        public async Task<UserTypeOut?> GetProfile(Guid userId)
        {
            // Intenta obtener el perfil desde la caché.
            var cachedProfile = await TryGetCachedProfile(userId);

            // Si se encuentra en la caché, devuélvelo.
            if (cachedProfile != null)
            {
                return cachedProfile;
            }

            // Si no se encuentra en la caché, obtén los datos del usuario y su media desde las bases de datos.
            var userSql = await _userRepository.GetAllDataAboutUserById(userId);
            var postUser = await _postRepository.GetAllPostsByIdAsync(userId.ToString());
            var reelUser = await _reelRepository.GetAllReelsByIdAsync(userId.ToString());
            var storyUser = await _storyRepository.GetAllStoriesByIdAsync(userId.ToString());

            // Si no se encuentra el usuario en la base de datos, devuelve null.
            if (userSql == null)
            {
                return null;
            }

            // Mapea los datos del usuario a un objeto UserTypeOut.
            var profile = UserMapper.MapUserEntityToUserTypeOut(userSql);

            // Mapea las publicaciones del usuario y asigna la lista al perfil.
            profile.Posts = postUser?.Select(p => PostMapper.MapPostEntityToPostTypeOut(p));
            profile.Reels = reelUser?.Select(r => ReelMapper.MapReelEntityToReelTypeOut(r));
            profile.Stories = storyUser?.Select(s => StoryMapper.MapStoryEntityToStoryTypeOut(s));

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
