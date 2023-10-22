using Instagram.Domain.Enums;

namespace Instagram.Domain.Entities.Notifications
{

    public class DeviceTokenEntity
    {
        public Guid UserId { get; set; }
        public string DeviceToken { get; set; } = null!;
        public DeviceEnum DeviceType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
