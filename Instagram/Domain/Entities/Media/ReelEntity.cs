using Instagram.Domain.Entities.Shared.Media;

namespace Instagram.Domain.Entities.Media
{
    public class ReelEntity
    {
        public string ReelId { get; set; } = null!;
        public Guid UserId { get; set; }
        public string Username { get; set; } = null!;
        public string? ImageProfile { get; set; }
        public string? Caption { get; set; }
        public DateTime DatePublication { get; set; }
        public string Url { get; set; } = null!;
        public LocationEntity? LocationReel { get; set; }
        public List<string>? Tags { get; set; }
        public long Likes { get; set; }
        public List<CommentEntity>? Comments { get; set; }
        public double Duration { get; set; } 
        public SongMediaEntity? Song { get; set; }
        public List<MentionEntity>? Mentions { get; set; }

    }
}
