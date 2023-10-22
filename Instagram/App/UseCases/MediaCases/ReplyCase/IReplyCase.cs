using Instagram.App.UseCases.MediaCases.Types.Shared.Media;
using Instagram.App.UseCases.MediaCases.Types.Shared.Media.Events;
using Instagram.App.UseCases.Types.Shared;

namespace Instagram.App.UseCases.MediaCases.ReplyCase
{
    public interface IReplyCase
    {
        public Task<ReplyEventType> CreateReply(ReplyType reply);
        public Task<ResponseType<string>> DeleteReply(ReplyType reply);
    }
}
