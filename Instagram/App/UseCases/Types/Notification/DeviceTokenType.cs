using Instagram.Domain.Enums;

namespace Instagram.App.UseCases.Types.Notification
{
    public class DeviceTokenType
    {
        public Guid UserId { get; set; }
        public string DeviceToken { get; set; } = null!;
        public DeviceEnum DeviceType { get; set; }
    }
}
