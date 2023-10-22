using Instagram.Domain.Entities.Media;

namespace Instagram.Domain.Repositories.Interfaces.Graph.Post
{
    public interface IPostNeo4jRepository
    {
        public Task<bool> CreatePostNodeAsync(PostEntity post);
        public Task DeletePostNodeAsync(string userId, string postId);
        public Task AddLikeAsync(string postId, string userId);
        public Task DeleteLikeAsync(string postId, string userId);
        public Task EditTagsAsync(string userId, string postId, List<string> tags);
    }
}
