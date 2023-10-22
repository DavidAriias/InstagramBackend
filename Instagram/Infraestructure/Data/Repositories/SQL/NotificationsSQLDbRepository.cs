using Instagram.Domain.Entities.Notifications;
using Instagram.Domain.Repositories.Interfaces.SQL.Notifications;
using Instagram.Infraestructure.Mappers.Notifications;
using Instagram.Infraestructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Instagram.Infraestructure.Data.Repositories.SQL
{
    public class NotificationsSQLDbRepository : INotificationsSQLDbRepository
    {
        private readonly InstagramContext _context;
        public NotificationsSQLDbRepository(
            InstagramContext context)
        {
            _context = context;
        }

        public async Task<List<string>?> GetDeviceTokensByUserId(Guid userId)
        {
            // Buscar tokens de dispositivo para el usuario especificado por su ID.
            var userDeviceTokens = await _context.UserDeviceTokens
                .Where(token => token.UserId == userId)
                .Select(token => token.DeviceToken)
                .ToListAsync();

            if (userDeviceTokens.Any())
            {
                // Devolver la lista de tokens de dispositivo si se encontraron.
                return userDeviceTokens;
            }

            // Si no se encontraron tokens de dispositivo, devolver null o una lista vacía, según tus necesidades.
            return null;
        }

        public async Task<bool> IsExistDeviceToken(string deviceTokenId)
        {
            // Realiza una consulta en la base de datos para verificar si el token existe.
            var existingToken = await _context.UserDeviceTokens
                .FirstOrDefaultAsync(token => token.DeviceToken == deviceTokenId);

            // Si el token existe, se considera que ya existe en la base de datos.
            return existingToken != null;
        }

        public async Task<bool> RegisterDeviceToken(DeviceTokenEntity deviceToken)
        {
            // Mapear la entidad de token de dispositivo a UserDeviceToken
            var tokenToDb = NotificationMapper.MapDeviceTokenEntityToUserDeviceToken(deviceToken);

            // Agregar el token a la base de datos
            await _context.UserDeviceTokens.AddAsync(tokenToDb);

            // Guardar los cambios en la base de datos
            var result = await _context.SaveChangesAsync();

            // Devolver true si se guardaron los cambios correctamente (result == 1)
            return result == 1;
        }

    }
}
