using Instagram.Domain.Repositories.Interfaces.Document.Story;
using Quartz;

namespace Instagram.Domain.Services
{
    public class StoryCleanupJob : IJob
    {
        private readonly IStoryMongoDbRepository _storyRepository;
        public StoryCleanupJob(IStoryMongoDbRepository storyRepository) 
        {
            _storyRepository = storyRepository;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            await _storyRepository.RemoveExpiredStories();
        }
    }
}
