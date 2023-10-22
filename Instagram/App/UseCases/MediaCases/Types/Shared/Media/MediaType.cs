using Instagram.Domain.Enums;

namespace Instagram.App.UseCases.MediaCases.Types.Shared.Media
{
    public class MediaType
    {
        public IFile Media { get; set; } = null!;
        public MediaEnum DataType { get; set; }

    }
}
