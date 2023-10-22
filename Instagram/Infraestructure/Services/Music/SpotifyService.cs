using Instagram.config.constants;
using Instagram.Domain.Entities;
using Instagram.Domain.Entities.Media;
using Instagram.Domain.Repositories.Interfaces.Music;
using SpotifyAPI.Web;

namespace Instagram.Infraestructure.Services.Music
{
    public class SpotifyApiRepository : ISpotifyApiRepository
    {
        private readonly SpotifyClient _spotifyClient;
        public SpotifyApiRepository()
        {
            _spotifyClient = new SpotifyClient(GetAccessToken());
        }

        protected SpotifyClientConfig GetAccessToken()
        {

            if (EnvironmentConfig.ClientId is null || EnvironmentConfig.ClientSecret is null)
            {
                throw new NullReferenceException("It's missing some credential for gettting Spotify's token");
            }

            //Generando token
            var config = SpotifyClientConfig
                        .CreateDefault()
                        .WithAuthenticator(new ClientCredentialsAuthenticator($"{EnvironmentConfig.ClientId}", $"{EnvironmentConfig.ClientSecret}"));

            return config;
        }

        public async Task<IEnumerable<SongServiceEntity>?> RecommendationsByTypeSong()
        {
            RecommendationsRequest request = new();

            List<SongServiceEntity> songs = new();

            var result = await _spotifyClient.Browse.GetRecommendations(request);

            if (result is not null)
            {

            }

            return songs;

        }

        public async Task<IEnumerable<SongServiceEntity>?> SearchSongByName(string keyword)
        {
            var request = new SearchRequest(SearchRequest.Types.Track, keyword);
            var result = await _spotifyClient.Search.Item(request);

            var songs = result.Tracks.Items!
                .Where(track => track.PreviewUrl != null && track.Album.Images.Count > 0)
                .Select(track => new SongServiceEntity
                {
                    ArtistName = track.Artists.First().Name,
                    Uri = track.Uri,
                    Duration = track.DurationMs,
                    PreviewUrl = track.PreviewUrl,
                    SongName = track.Name,
                    ImagesAlbum = track.Album.Images,
                    AlbumName = track.Album.Name,
                })
                .ToList();

            return songs.Any() ? songs : null;
        }


    }
}
