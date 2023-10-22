using Instagram.App.UseCases.MediaCases.Types.Shared.Media;
using Instagram.App.UseCases.Types.Shared;

namespace Instagram.App.UseCases.MediaCases.CaptionCase
{
    public interface ICaptionCase
    {
        public Task<ResponseType<string>> UpdateCaption(CaptionTypeIn captionType);
    }
}
