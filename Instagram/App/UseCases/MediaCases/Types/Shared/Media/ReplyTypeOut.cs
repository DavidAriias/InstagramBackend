namespace Instagram.App.UseCases.MediaCases.Types.Shared.Media
{
    public class ReplyTypeOut
    {
        public string ReplyId { get; set; } = null!;
        public Guid UserId { get; set; }
        public string Text { get; set; } = null!;
        public DateTime Date { get; set; }
    }
}
