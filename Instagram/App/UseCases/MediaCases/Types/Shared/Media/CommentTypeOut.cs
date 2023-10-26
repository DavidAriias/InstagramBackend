namespace Instagram.App.UseCases.MediaCases.Types.Shared.Media
{
    public class CommentTypeOut
    {
        public Guid UserId { get; set; }
        public string Text { get; set; } = null!;
        public DateTime Date { get; set; }

        [UsePaging]
        public List<ReplyTypeOut>? Replies { get; set; }
    }
}
