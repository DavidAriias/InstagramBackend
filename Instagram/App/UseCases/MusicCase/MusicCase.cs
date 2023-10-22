using Instagram.Domain.Repositories.Interfaces.Cache;
using Instagram.Domain.Repositories.Interfaces.Music;
using Instagram.Infraestructure.Mappers.Song;
using Newtonsoft.Json;

namespace Instagram.App.UseCases.MusicCase
{
    public class MusicCase : IMusicCase
    {
        private readonly ISpotifyApiRepository _musicRepository;
        private readonly IRedisRepository _redisRepository;
        public MusicCase(ISpotifyApiRepository musicRepository, IRedisRepository redisRepository)
        { 
            _musicRepository = musicRepository;
            _redisRepository = redisRepository;           
        }

        public async Task<IEnumerable<SongSearchType>?> SearchSongByName(string songName)
        {
            // Intenta recuperar las canciones desde la caché
            var cachedSongs = await TryGetCachedSongs(songName);

            if (cachedSongs is not null)
            {
                return cachedSongs;
            }

            // Busca en el servicio de música
            var response = await _musicRepository.SearchSongByName(songName);

            if (response is not null)
            {
                var songs = SongMapper.MapSongSearchEntityToSongSearchType(response);

                // Guarda en caché las canciones obtenidas
                await CacheSongs(songName, songs);

                return songs;
            }

            return null;
        }

        private async Task<IEnumerable<SongSearchType>?> TryGetCachedSongs(string songName)
        {
            var cache = await _redisRepository.GetAsync($"song-name:{songName}");

            if (cache is not null)
            {
                return JsonConvert.DeserializeObject<IEnumerable<SongSearchType>>(cache);
            }
            return null;
        }
        private async Task CacheSongs(string songName, IEnumerable<SongSearchType> songs)
        {
            await _redisRepository.SetAsync($"song-name:{songName}", JsonConvert.SerializeObject(songs), TimeSpan.FromMinutes(2));
        }
    }
}
