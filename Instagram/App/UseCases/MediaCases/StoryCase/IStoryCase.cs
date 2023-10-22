using Instagram.App.UseCases.MediaCases.Types.Stories;
using Instagram.App.UseCases.Types.Feed;
using Instagram.App.UseCases.Types.Shared;

namespace Instagram.App.UseCases.MediaCases.StoryCase
{
    public interface IStoryCase
    {
        public Task<StoryEventType> CreateStory(StoryTypeIn storyType);
        public Task<ResponseType<string>> DeleteStory(string storyId, Guid userId);
        public Task<IReadOnlyList<FeedType<StoryTypeOut>>> GetStoriesByUserId(Guid userId);
    }
}
