namespace Instagram.Domain.Entities.Shared.Media
{
    public class CommentEntity
    {
        public Guid UserId { get; set; }
        public string Text { get; set; } = null!;
        public DateTime Date { get; set; }
        public List<ReplyEntity>? Replies { get; set; }
    }
}
