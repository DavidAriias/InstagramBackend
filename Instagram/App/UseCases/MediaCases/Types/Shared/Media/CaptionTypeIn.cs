using Instagram.Domain.Enums;

namespace Instagram.App.UseCases.MediaCases.Types.Shared.Media
{
    public class CaptionTypeIn
    {
        public ContentEnum ContentType { get; set; }
        public string Caption { get; set; } = null!;
        public string MediaId { get; set; } = null!;
    }
}
