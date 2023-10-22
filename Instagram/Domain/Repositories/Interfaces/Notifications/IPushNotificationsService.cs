using Instagram.Domain.Entities.Notifications;

namespace Instagram.Domain.Repositories.Interfaces.Notifications
{
    public interface IPushNotificationsService
    {
        public Task OnSendNotification(NotificationEntity notificationEntity);
        public Task OnSendMultiplyNotifications(List<NotificationEntity> notificationEntities);
    }
}
