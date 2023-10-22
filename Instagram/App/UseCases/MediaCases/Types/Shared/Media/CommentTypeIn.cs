using Instagram.Domain.Enums;

namespace Instagram.App.UseCases.MediaCases.Types.Shared.Media
{
    public class CommentTypeIn
    {
        public ContentEnum ContentType { get; set; }
        public string? Comment { get; set; }
        public Guid UserId { get; set; }
        public string MediaId { get; set; } = null!;
        public string? CommentId { get; set; }
    }
}
