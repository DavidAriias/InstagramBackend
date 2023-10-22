using Instagram.App.UseCases.Types.Search;
using Instagram.Domain.Repositories.Interfaces.Cache;
using Instagram.Domain.Repositories.Interfaces.SQL.Search;
using Instagram.Infraestructure.Mappers.Search;
using Newtonsoft.Json;

namespace Instagram.App.UseCases.SearchCase
{
    public class SearchCase : ISeachCase
    {
        private readonly ISearchSQLDbRepository _searchRepository;
        private readonly IRedisRepository _redisRepository;
        public SearchCase(ISearchSQLDbRepository searchRepository, IRedisRepository redisRepository) 
        {
            _searchRepository = searchRepository;
            _redisRepository = redisRepository;
        }

        public async Task<IReadOnlyCollection<SearchType>?> SearchUsers(string input)
        {
            // Realiza una búsqueda de usuarios por nombre de usuario y nombre en la base de datos.
            var cachedSearch = await TryGetSearchUsers(input);

            if (cachedSearch != null)
            {
                return cachedSearch;
            }

            var result = await _searchRepository.SearUsersByAnInput(input);

            if (result is not null)
            {

                // Mapea los resultados a objetos UserTypeOut, convierte la lista en solo lectura y devuelve.
                var mappedUsers = result.Select(s => SearchMapper.MapSearchEntityToSearchType(s))
                    .ToList()
                    .AsReadOnly();

                await CacheSearch(input, mappedUsers);

                return mappedUsers;
            }

            return null;
        }
        private async Task<IReadOnlyCollection<SearchType>?> TryGetSearchUsers(string input)
        {
            // Intenta obtener el perfil desde la caché utilizando la clave única del usuario.
            var cache = await _redisRepository.GetAsync($"search:{input}");

            // Si se encuentra en la caché, deserializa y devuelve el perfil.
            return cache != null ? JsonConvert.DeserializeObject<IReadOnlyCollection<SearchType>?>(cache) : null;
        }

        private async Task CacheSearch(string input, IReadOnlyCollection<SearchType> search)
        {
            // Almacena el perfil en la caché con una clave única durante 5 minutos.
            await _redisRepository.SetAsync($"search:{input}", JsonConvert.SerializeObject(search),
                TimeSpan.FromMinutes(5));
        }

    }
}
