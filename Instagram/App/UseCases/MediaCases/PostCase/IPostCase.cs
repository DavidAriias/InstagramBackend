using Instagram.App.UseCases.MediaCases.Types.Posts;
using Instagram.App.UseCases.Types.Feed;
using Instagram.App.UseCases.Types.Shared;

namespace Instagram.App.UseCases.MediaCases.PostCase
{
    public interface IPostCase
    {
        public Task<PostEventType> CreatePostAsync(PostTypeIn postIn);
        public Task<ResponseType<string>> DeletePostAsync(string postId, Guid userId);
        public Task<IReadOnlyList<FeedType<PostTypeOut>>> GetFeedPostByUserId(Guid userId);

    }
}
