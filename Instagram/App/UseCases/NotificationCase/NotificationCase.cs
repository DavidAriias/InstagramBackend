using Instagram.App.UseCases.Types.Notification;
using Instagram.App.UseCases.Types.Shared;
using Instagram.Domain.Entities.Notifications;
using Instagram.Domain.Repositories.Interfaces.Graph.User;
using Instagram.Domain.Repositories.Interfaces.Notifications;
using Instagram.Domain.Repositories.Interfaces.SQL.Notifications;
using Instagram.Infraestructure.Mappers.Notifications;

namespace Instagram.App.UseCases.NotificationCase
{
    public class NotificationCase : INotificationCase
    {
        private readonly IUserNeo4jRepository _userRepository;
        private readonly INotificationsSQLDbRepository _notificationsRepository;
        private readonly IPushNotificationsService _postNotificationsService;
        private readonly ILogger<NotificationCase> _logger;
        public NotificationCase(
            IUserNeo4jRepository userRepository,
            INotificationsSQLDbRepository notificationsSQLDbRepository, 
            IPushNotificationsService postNotificationsService,
            ILogger<NotificationCase> logger
            ) 
        {
            _userRepository = userRepository;
            _notificationsRepository = notificationsSQLDbRepository;
            _postNotificationsService = postNotificationsService;
            _logger = logger;
        }

        public async Task<ResponseType<string>> RegisterDeviceToken(DeviceTokenType deviceToken)
        {
            // Comprueba si el token de dispositivo ya existe en la base de datos.
            bool isTokenAlreadyRegistered = await _notificationsRepository.IsExistDeviceToken(deviceToken.DeviceToken);

            bool isSaved = false;

            // Si el token no existe, regístralo en la base de datos.
            if (!isTokenAlreadyRegistered)
            {
                var deviceTokenToDb = NotificationMapper.MapDeviceTokenTypeToDeviceTokenEntity(deviceToken);
                isSaved = await _notificationsRepository.RegisterDeviceToken(deviceTokenToDb);
            }

            // Devuelve una respuesta según si se guardó o no el token de dispositivo.
            if (isSaved)
            {
                return ResponseType<string>.CreateSuccessResponse(
                    null,
                    System.Net.HttpStatusCode.NoContent
                    ,"Device token saved successfully");
            }
            else
            {
                return ResponseType<string>.CreateErrorResponse(
                    System.Net.HttpStatusCode.InternalServerError,
                    "There was an error while saving the device token"
                );
            }
        }


        public async Task SendPushNotificationToFollowers(string message, Guid userId)
        {
            // Lista para almacenar las notificaciones a enviar.
            var notificationsToSend = new List<NotificationEntity>();

            try
            {
                // Obtener la lista de usuarios seguidos por el usuario actual.
                var followedUsers = await _userRepository.UsersFollowedByOthers(userId.ToString());

                foreach (var followedUser in followedUsers)
                {
                    // Obtener los tokens de dispositivo de los usuarios seguidos.
                    var deviceTokens = await _notificationsRepository.GetDeviceTokensByUserId(Guid.Parse(followedUser));

                    if (deviceTokens != null && deviceTokens.Any())
                    {
                        // Crear una notificación para cada token de dispositivo.
                        foreach (var deviceToken in deviceTokens)
                        {
                            var notification = new NotificationEntity
                            {
                                Body = message,
                                DeviceToken = deviceToken
                            };

                            // Agregar la notificación a la lista.
                            notificationsToSend.Add(notification);
                        }
                    }
                }

                // Enviar múltiples notificaciones.
                await _postNotificationsService.OnSendMultiplyNotifications(notificationsToSend);
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir durante el proceso.
                _logger.LogError(ex, "Error while sending notifications to followers.");
            }
        }

        public async Task SendPushNotificationToUser(string message, Guid userId)
        {
            try
            {
                // Obtener los tokens de dispositivo del usuario.
                var deviceTokens = await _notificationsRepository.GetDeviceTokensByUserId(userId);

                var notificationsToSend = new List<NotificationEntity>();

                if (deviceTokens is null) return;

                foreach (var deviceToken in deviceTokens)
                {
                    var notification = new NotificationEntity
                    {
                        Body = message,
                        DeviceToken = deviceToken
                    };

                    notificationsToSend.Add(notification);
                }

                // Enviar múltiples notificaciones.
                await _postNotificationsService.OnSendMultiplyNotifications(notificationsToSend);
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir durante el proceso.
                _logger.LogError(ex, "Error while sending push notification to the user.");
            }
        }

    }
}
