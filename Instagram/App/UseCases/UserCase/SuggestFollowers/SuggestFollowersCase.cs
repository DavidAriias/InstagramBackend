using Instagram.App.UseCases.UserCase.Types;
using Instagram.Domain.Repositories.Interfaces.Cache;
using Instagram.Domain.Repositories.Interfaces.Graph.User;
using Instagram.Domain.Repositories.Interfaces.SQL.User;
using Newtonsoft.Json;

namespace Instagram.App.UseCases.UserCase.SuggestFollowers
{
    public class SuggestFollowersCase : ISuggestFollowersCase
    {
        private readonly IUserNeo4jRepository _userNeo4JRepository;
        private readonly IRedisRepository _redisRepository;
        private readonly IUserSQLDbRepository _userSQLRepository;
        private readonly ILogger<SuggestFollowersCase> _logger;
        public SuggestFollowersCase(
            IUserNeo4jRepository userNeo4JRepository,
            IRedisRepository redisRepository,
            IUserSQLDbRepository userSQLRepository,
            ILogger<SuggestFollowersCase> logger
            ) 
        {
            _userNeo4JRepository = userNeo4JRepository;
            _redisRepository = redisRepository;
            _userSQLRepository = userSQLRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<FollowerType>> GetSuggestionFollowers(Guid userId)
        {
            try
            {
                var cachedSuggestions = await TryGetCachedSuggestions(userId);

                if (cachedSuggestions is not null) return cachedSuggestions;

                // Obtener los IDs de usuarios sugeridos para seguir desde Neo4J.
                var suggestionIds = await _userNeo4JRepository.SuggestionFollowers(userId.ToString());

                var suggestions = new List<FollowerType>();

                foreach (var suggestedUserId in suggestionIds)
                {
                    var userData = await _userSQLRepository.FindUserById(Guid.Parse(suggestedUserId));

                    if (userData != null)
                    {
                        var suggestion = FollowerType.Create(
                            Guid.Parse(suggestedUserId), 
                            userData.Imageprofile, 
                            userData.Username);

                        suggestions.Add(suggestion);
                    }
                }

                // Filtrar y quitar los elementos nulos.
                suggestions = suggestions.Where(suggestion => suggestion != null).ToList();

                await CacheSuggestions(userId,suggestions);
                // Devolver la lista de seguidores sugeridos.
                return suggestions;
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir durante la obtención de seguidores sugeridos y registrar un error.
                _logger.LogError(ex, "Error while fetching suggestion followers for the user.");
            }

            // En caso de error, devolver una lista vacía.
            return new List<FollowerType>();
        }

        private async Task<IEnumerable<FollowerType>?> TryGetCachedSuggestions(Guid userId)
        {
            // Intenta obtener el perfil desde la caché utilizando la clave única del usuario.
            var cache = await _redisRepository.GetAsync($"suggestions:{userId}");

            // Si se encuentra en la caché, deserializa y devuelve los seguidos.
            return cache != null ? JsonConvert
                .DeserializeObject<IEnumerable<FollowerType>?>(cache) : null;
        }

        private async Task CacheSuggestions(Guid userId, IEnumerable<FollowerType> suggestions)
        {
            // Almacena el perfil en la caché con una clave única durante 5 minutos.
            await _redisRepository.SetAsync($"suggestions:{userId}", JsonConvert.SerializeObject(suggestions),
                TimeSpan.FromMinutes(5));
        }
    }
}
