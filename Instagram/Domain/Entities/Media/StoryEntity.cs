using Instagram.Domain.Entities.Shared.Media;
using MongoDB.Bson;

namespace Instagram.Domain.Entities.Media
{
    public class StoryEntity
    {
        public string StoryId { get; set; } = null!;
        public Guid UserId { get; set; } 
        public DateTime PostDate { get; set; } 
        public LocationEntity? LocationStory { get; set; } 
        public string MediaUrl { get; set; } = null!;
        public SongMediaEntity?  SongMedia { get; set; }
    }
}
