using Instagram.App.UseCases.MediaCases.Types.Shared.Media;

namespace Instagram.App.UseCases.MusicCase
{
    public class SongMediaType
    {
        public string Title { get; set; } = null!;
        public string Artist { get; set; } = null!;
        public string Url { get; set; } = null!;
        public string TotalDuration { get; set; } = null!;
        public DurationSelectionType Selection { get; set; } = null!;
    }
}
