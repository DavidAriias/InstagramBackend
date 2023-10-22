using Instagram.Domain.Enums;

namespace Instagram.App.UseCases.MediaCases.Types.Shared.Media
{
    public class TagsTypeIn
    {
        public List<string> Tags { get; set; } = null!;
        public string MediaId { get; set; } = null!;
        public Guid UserId { get; set; }
        public ContentEnum ContentType { get; set; }
    }
}
