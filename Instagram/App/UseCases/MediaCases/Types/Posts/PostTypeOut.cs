using Instagram.App.UseCases.MediaCases.Types.Shared.Media;
using Instagram.App.UseCases.MusicCase;

namespace Instagram.App.UseCases.MediaCases.Types.Posts
{
    public class PostTypeOut
    {
        public string Username { get; set; } = null!;
        public string? ImageProfile { get; set; }
        public string PostId { get; set; } = null!;
        public List<string> Images { get; set; } = null!;
        public string? Caption { get; set; }
        public DateTime DatePublication { get; set; }
        public long Likes { get; set; }

        [UsePaging]
        public List<CommentTypeOut>? Comments { get; set; }
        public List<string>? Hashtags { get; set; }
        public LocationType? LocationPost { get; set; }
        public SongMediaType? Song { get; set; }
        public List<MentionType>? Mentions { get; set; }
    }
}
