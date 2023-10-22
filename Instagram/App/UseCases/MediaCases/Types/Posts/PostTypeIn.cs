using Instagram.App.UseCases.MediaCases.Types.Shared.Media;
using Instagram.App.UseCases.MusicCase;

namespace Instagram.App.UseCases.MediaCases.Types.Posts
{
    public class PostTypeIn
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = null!;
        public LocationType? Location { get; set; }
        public SongMediaType? Music { get; set; }
        public string? Caption { get; set; }
        public List<string>? Tags { get; set; }
        public List<MediaType> Media { get; set; } = null!;
        public List<MentionType>? Mentions { get; set; }
    }
}
