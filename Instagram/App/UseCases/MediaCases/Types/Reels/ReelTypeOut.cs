using Instagram.App.UseCases.MediaCases.Types.Shared.Media;
using Instagram.App.UseCases.MusicCase;

namespace Instagram.App.UseCases.MediaCases.Types.Reels
{
    public class ReelTypeOut
    {
        public string ReelId { get; set; } = null!;
        public string? Caption { get; set; }
        public DateTime DatePublication { get; set; }
        public string Url { get; set; } = null!;
        public LocationType? LocationReel { get; set; }
        public List<string>? Tags { get; set; }
        public long Likes { get; set; }

        [UsePaging]
        public List<CommentTypeOut>? Comments { get; set; }
        public double Duration { get; set; }
        public SongMediaType? Song { get; set; }
        public List<MentionType>? Mentions { get; set; }
    }
}
