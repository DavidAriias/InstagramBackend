using Instagram.Domain.Enums;

namespace Instagram.App.UseCases.MediaCases.Types.Shared.Media
{
    public class ReplyType
    {
        public string? ReplyId { get; set; }
        public Guid UserId { get; set; }
        public string CommentId { get; set; } = null!;
        public string MediaId { get; set; } = null!;
        public string? Text { get; set; }
        public ContentEnum ContentType { get; set; }

    }
}
