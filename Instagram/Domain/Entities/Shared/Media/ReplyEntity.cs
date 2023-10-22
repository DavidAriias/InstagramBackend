namespace Instagram.Domain.Entities.Shared.Media
{
    public class ReplyEntity
    {
        public string ReplyId { get; set; } = null!;

        public string UserId { get; set; } = null!;

        public string Text { get; set; } = null!;

        public DateTime Date { get; set; }
    }
}
