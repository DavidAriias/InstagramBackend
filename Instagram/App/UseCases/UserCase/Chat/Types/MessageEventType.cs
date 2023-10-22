namespace Instagram.App.UseCases.UserCase.Chat.Types
{
    public class MessageEventType
    {
        public Guid TextId { get; set; }
        public string Context { get; set; } = null!;
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
