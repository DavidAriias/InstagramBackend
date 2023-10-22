using Instagram.App.UseCases.MediaCases.Types.Shared.Media;
using Instagram.App.UseCases.MediaCases.Types.Shared.Media.Events;
using Instagram.App.UseCases.Types.Shared;

namespace Instagram.App.UseCases.MediaCases.LikeCase
{
    public interface ILikeCase
    {
        public Task<LikeEventType> AddLikeToMedia(LikeTypeIn typeIn);
        public Task<ResponseType<string>> RemoveLikeToMedia(LikeTypeIn typeIn);
    }
}
