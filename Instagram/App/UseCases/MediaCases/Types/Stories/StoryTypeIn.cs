using Instagram.App.UseCases.MediaCases.Types.Shared.Media;
using Instagram.App.UseCases.MusicCase;

namespace Instagram.App.UseCases.MediaCases.Types.Stories
{
    public class StoryTypeIn
    {
        public LocationType? Location { get; set; }
        public SongMediaType? Music { get; set; }
        public MediaType Media { get; set; } = null!;
        public Guid UserId { get; set; }
        public string Username { get; set; } = null!;
        public long Duration { get; set; }
    }
}
