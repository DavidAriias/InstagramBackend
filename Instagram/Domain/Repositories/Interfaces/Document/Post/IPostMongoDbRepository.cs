using Instagram.Domain.Entities.Media;
using Instagram.Domain.Entities.Shared.Media;

namespace Instagram.Domain.Repositories.Interfaces.Document.Post
{
    public interface IPostMongoDbRepository
    {
        public Task<string?> CreatePostAsync(PostEntity post);
        public Task<bool> EditCaptionAsync(string caption, string postId);
        public Task<bool> EditLocationAsync(LocationEntity location, string postId);
        public Task<bool> EditTagsAsync(List<string> tags, string postId);
        public Task<bool> DeletePostAsync(string postId);
        public Task<string?> AddCommentAsync(CommentEntity commentEntity, string postId);
        public Task<bool> DeleteCommentAsync(string userId, string mediaId, string commentId);
        public Task<bool> AddReplyAsync(ReplyEntity entity, string postId, string commentId);
        public Task<bool> DeleteReplyAsync(string postId, string commentId, ReplyEntity replyEntity);
        public Task<IEnumerable<PostEntity>?> GetAllPostsByIdAsync(string userId);
        public Task<bool> AddLikeAsync(string mediaId);
        public Task<bool> DeleteLikeAsync(string mediaId);
        public Task<PostEntity?> FindPostByIdAsync(string postId);
        public Task<IEnumerable<PostEntity>?> GetLastPostsIn3DaysAsync(string userId);
    }
}
