namespace Instagram.Domain.Entities
{
    public class MediaMessageEntity
    {
        public Guid MediaId { get; set; }
        public string MediaUrl { get; set; } = null!;
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
