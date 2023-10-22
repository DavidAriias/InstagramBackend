using SpotifyAPI.Web;

namespace Instagram.Domain.Entities.Shared.Media
{
    public class SongMediaEntity
    {
        public string Title { get; set; } = null!;
        public string Artist { get; set; } = null!;
        public string Url { get; set; } = null!;
        public string TotalDuration { get; set; } = null!;
        public DurationSelectionEntity Selection { get; set; } = null!;
    }
}
