using Instagram.Domain.Enums;

namespace Instagram.App.UseCases.MediaCases.Types.Shared.Media
{
    public class MediaType
    {
        public IFormFile Media { get; set; } = null!;
        public MediaEnum DataType { get; set; }

    }
}
