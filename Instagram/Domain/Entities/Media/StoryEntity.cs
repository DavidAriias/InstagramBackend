using Instagram.Domain.Entities.Shared.Media;


namespace Instagram.Domain.Entities.Media
{
    public class StoryEntity
    {
        public string StoryId { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string? ImageProfile { get; set; }
        public Guid UserId { get; set; } 
        public DateTime PostDate { get; set; } 
        public LocationEntity? LocationStory { get; set; } 
        public string MediaUrl { get; set; } = null!;
        public SongMediaEntity?  SongMedia { get; set; }
        public long Duration { get; set; }
    }
}
