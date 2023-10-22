using Instagram.Domain.Entities.Media;

namespace Instagram.Domain.Repositories.Interfaces.Graph.Reel
{
    public interface IReelNeo4jRepository
    {
        public Task<bool> CreateReelNodeAsync(ReelEntity reelEntity);
        public Task DeleteReelNodeAsync(string userId, string reelId);
        public Task AddLikeAsync(string reelId, string userId);
        public Task DeleteLikeAsync(string reelId, string userId);
        public Task EditTagsAsync(string userId, string reelId, List<string> tags);
    }
}
