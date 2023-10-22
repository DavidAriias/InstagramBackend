using Instagram.Domain.Entities.Media;
using Instagram.Domain.Entities.Shared.Media;

namespace Instagram.Domain.Repositories.Interfaces.Document.Reel
{
    public interface IReelMongoDbRepository
    {
        public Task<string?> CreateReelAsync(ReelEntity reelEntity);
        public Task<bool> DeleteReelAsync(string reelId);
        public Task<bool> EditCaptionAsync(string caption, string reelId);
        public Task<bool> EditLocationAsync(LocationEntity location, string reelId);
        public Task<bool> EditTagsAsync(List<string> tags, string reelId);
        public Task<string?> AddCommentAsync(CommentEntity commentEntity, string reelId);
        public Task<bool> AddReplyAsync(ReplyEntity entity, string mediaId, string commentId);
        public Task<bool> DeleteReplyAsync(string reelId, string commentId, ReplyEntity replyEntity);
        public Task<IEnumerable<ReelEntity>?> GetAllReelsByIdAsync(string userId);
        public Task<bool> DeleteCommentAsync(string userId, string mediaId, string commentId);
        public Task<bool> AddLikeAsync(string mediaId);
        public Task<bool> DeleteLikeAsync(string mediaId);
        public Task<ReelEntity?> FindReelById(string reelId);
        public Task<IEnumerable<ReelEntity>?> GetLastReelsIn3Days(string userId);

    }
}
