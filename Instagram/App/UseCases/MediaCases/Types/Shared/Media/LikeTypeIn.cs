using Instagram.Domain.Enums;

namespace Instagram.App.UseCases.MediaCases.Types.Shared.Media
{
    public class LikeTypeIn
    {
        public ContentEnum ContentType { get; set; }
        public Guid UserId { get; set; }
        public string MediaId { get; set; } = null!;

    }
}
