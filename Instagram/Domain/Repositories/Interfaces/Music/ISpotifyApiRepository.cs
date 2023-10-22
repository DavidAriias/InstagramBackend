using Instagram.Domain.Entities;

namespace Instagram.Domain.Repositories.Interfaces.Music
{
    public interface ISpotifyApiRepository
    {
        public Task<IEnumerable<SongServiceEntity>?> SearchSongByName(string keyword);

        public Task<IEnumerable<SongServiceEntity>?> RecommendationsByTypeSong();

    }
}
