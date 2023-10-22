using Instagram.Domain.Entities.Shared.Media;

namespace Instagram.Domain.Entities.Media
{
    public class PostEntity
    {
        public string PostId { get; set; } = null!;
        public Guid UserId { get; set; }
        public List<string> Images { get; set; } = null!;
        public string? Caption { get; set; }
        public DateTime DatePublication { get; set; }
        public long Likes { get; set; }
        public List<CommentEntity>? Comments { get; set; }
        public List<string>? Hashtags { get; set; }
        public LocationEntity? LocationPost { get; set; }
        public SongMediaEntity? Song { get; set; }
        public List<MentionEntity>? Mentions { get; set; }
    }
}
