using Instagram.App.UseCases.Types.Notification;
using Instagram.Domain.Entities.Notifications;
using Instagram.Infraestructure.Data.Models.SQL;

namespace Instagram.Infraestructure.Mappers.Notifications
{
    public static class NotificationMapper
    {
        public static UserDeviceToken MapDeviceTokenEntityToUserDeviceToken(DeviceTokenEntity deviceEntity) => new()
        {
            CreatedAt = deviceEntity.CreatedAt,
            DeviceToken = deviceEntity.DeviceToken,
            DeviceType = deviceEntity.DeviceToken,
            UserId = deviceEntity.UserId
        };

        public static DeviceTokenEntity MapDeviceTokenTypeToDeviceTokenEntity(DeviceTokenType deviceType) => new()
        {
            DeviceToken = deviceType.DeviceToken,
            DeviceType = deviceType.DeviceType,
            UserId = deviceType.UserId
        };
    }
}
