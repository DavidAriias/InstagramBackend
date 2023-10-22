namespace Instagram.Domain.Entities.Notifications
{
    public class NotificationEntity
    {
        public string Body { get; set; } = null!;
        public string DeviceToken { get; set; } = null!;
        public string? ImageUrl { get; set; }

    }
}
