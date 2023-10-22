using Instagram.App.UseCases.Types.Notification;
using Instagram.App.UseCases.Types.Shared;

namespace Instagram.App.UseCases.NotificationCase
{
    public interface INotificationCase
    {
        public Task<ResponseType<string>> RegisterDeviceToken(DeviceTokenType deviceToken);
        public Task SendPushNotificationToFollowers(string message, Guid userId);
        public Task SendPushNotificationToUser(string message, Guid userId);
    }
}
