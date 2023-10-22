using Instagram.config.constants;
using Instagram.Domain.Entities.Notifications;
using Instagram.Domain.Repositories.Interfaces.Notifications;
using Microsoft.Azure.NotificationHubs;

namespace Instagram.Infraestructure.Services.PushNotifications
{
    public class PushNotificationService : IPushNotificationsService
    {
        private readonly NotificationHubClient _notificationHub;
        private readonly ILogger<PushNotificationService> _logger;
        public PushNotificationService(ILogger<PushNotificationService> logger) 
        {
            _logger = logger;
            _notificationHub = NotificationHubClient
                .CreateClientFromConnectionString(
                EnvironmentConfig.ConnectionStringNotification,
                EnvironmentConfig.NotificationHubName);
        }

        public async Task OnSendNotification(NotificationEntity notificationEntity)
        {
            // Define la notificación push
            var notification = new Dictionary<string, string>
            {
                { "title", "Instagram" },
                { "body", notificationEntity.Body },
                { "icon", "https://www.pngall.com/wp-content/uploads/5/Instagram-Logo-PNG-Free-Download.png" },
                { "image", notificationEntity.ImageUrl ?? "" }
            };

            const int maxRetries = 3;
            int retryCount = 0;

            while (retryCount < maxRetries)
            {
                try
                {
                    // Intenta enviar la notificación
                    var outcome = await _notificationHub.SendTemplateNotificationAsync(notification, notificationEntity.DeviceToken);

                    if (outcome != null && !(outcome.State == NotificationOutcomeState.Abandoned || outcome.State == NotificationOutcomeState.Unknown))
                    {
                        _logger.LogInformation($"Notification sent to the '{notificationEntity.DeviceToken}' successfully.");
                        return; // Si se envía con éxito, sal del bucle.
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error sending the notification: {ex.Message}");
                }

                retryCount++;

                if (retryCount < maxRetries)
                {
                    _logger.LogWarning($"Retrying notification send to '{notificationEntity.DeviceToken}'. Attempt {retryCount} of {maxRetries}.");
                }
                else
                {
                    _logger.LogError($"Notification could not be sent to '{notificationEntity.DeviceToken}' after {maxRetries} retries.");
                }
            }
        }


        public async Task OnSendMultiplyNotifications(List<NotificationEntity> notificationEntities)
        {
            foreach (var notificationEntity in notificationEntities)
            {
                const int maxRetries = 3;
                int retryCount = 0;
                bool notificationSent = false;

                while (!notificationSent && retryCount < maxRetries)
                {
                    try
                    {
                        // Define la notificación push y el token del dispositivo
                        var notification = new Dictionary<string, string>
                        {
                            { "title", "Instagram" }, // Título de la notificación
                            { "body", notificationEntity.Body }, // Cuerpo de la notificación
                            { "icon", "https://www.pngall.com/wp-content/uploads/5/Instagram-Logo-PNG-Free-Download.png" }, // Icono de la notificación (puedes especificar una URL a una imagen)
                            { "image", notificationEntity.ImageUrl ?? "" }, // Imagen de la notificación (puedes especificar una URL a una imagen)
                        };

                        var token = notificationEntity.DeviceToken; // Asume que el token se encuentra en la entidad.

                        // Intenta enviar la notificación
                        await _notificationHub.SendTemplateNotificationAsync(notification, token);
                        notificationSent = true;
                        _logger.LogInformation($"Notification sent to device with token: {token}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error sending notification: {ex.Message}");
                        retryCount++;
                    }
                }

                if (!notificationSent)
                {
                    _logger.LogWarning($"Notification not sent to device with token: {notificationEntity.DeviceToken} after {maxRetries} retries.");
                    // Puedes registrar o manejar la notificación no enviada de alguna otra manera.
                }
            }
        }

    }
}
