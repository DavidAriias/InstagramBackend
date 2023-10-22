using Instagram.App.UseCases.MediaCases.Types.Shared.Media;
using Instagram.App.UseCases.MusicCase;

namespace Instagram.App.UseCases.MediaCases.Types.Stories
{
    public class StoryTypeOut
    {
        public string StoryId { get; set; } = null!;
        public DateTime PostDate { get; set; }
        public LocationType? LocationStory { get; set; }
        public long Duration { get; set; }
        public string MediaUrl { get; set; } = null!;
        public SongMediaType? SongMedia { get; set; }
    }
}
