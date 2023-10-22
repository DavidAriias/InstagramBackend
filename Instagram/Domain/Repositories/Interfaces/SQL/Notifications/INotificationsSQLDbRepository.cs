using Instagram.Domain.Entities.Notifications;

namespace Instagram.Domain.Repositories.Interfaces.SQL.Notifications
{
    public interface INotificationsSQLDbRepository
    {
        public Task<List<string>?> GetDeviceTokensByUserId(Guid userId);
        public Task<bool> RegisterDeviceToken(DeviceTokenEntity deviceToken);
        public Task<bool> IsExistDeviceToken(string deviceTokenId);

    }
}
