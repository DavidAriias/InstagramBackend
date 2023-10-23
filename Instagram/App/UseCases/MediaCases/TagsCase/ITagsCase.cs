using Instagram.App.UseCases.MediaCases.Types.Shared.Media;
using Instagram.App.UseCases.Types.Shared;

namespace Instagram.App.UseCases.MediaCases.TagsCase
{
    public interface ITagsCase
    {
        public Task<ResponseType<IReadOnlyList<string>>> UpdateTagsAsync(TagsTypeIn tagsType);

    }
}
