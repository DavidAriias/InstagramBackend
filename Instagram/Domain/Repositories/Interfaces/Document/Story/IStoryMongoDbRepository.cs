using Instagram.Domain.Entities.Media;

namespace Instagram.Domain.Repositories.Interfaces.Document.Story
{
    public interface IStoryMongoDbRepository
    {
        public Task<string?> CreateStoryAsync(StoryEntity storyEntity);
        public Task<bool> DeleteStoryAsync(string storyId);
        public Task<IEnumerable<StoryEntity>?> GetAllStoriesByIdAsync(string userId);
        public Task<StoryEntity?> FindStoryById(string storyId);
        public Task RemoveExpiredStories();
    }
}
