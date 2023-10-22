using Instagram.App.UseCases.MediaCases.Types.Shared.Media;
using Instagram.App.UseCases.MediaCases.Types.Shared.Media.Events;
using Instagram.App.UseCases.Types.Shared;

namespace Instagram.App.UseCases.MediaCases.CommentCase
{
    public interface ICommentCase
    {
        public Task<CommentEventType> CreateComment(CommentTypeIn commentType);
        public Task<ResponseType<string>> DeleteComment(CommentTypeIn commentType);
    }
}
