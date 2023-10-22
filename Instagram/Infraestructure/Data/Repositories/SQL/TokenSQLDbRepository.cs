using Instagram.Domain.Entities.Auth;
using Instagram.Domain.Repositories.Interfaces.SQL.Token;
using Instagram.Infraestructure.Mappers.Auth;
using Instagram.Infraestructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Instagram.Infraestructure.Data.Repositories.SQL
{
    public class TokenSQLDbRepository : ITokenSQLDbRepository
    {
        private readonly InstagramContext _context;
        public TokenSQLDbRepository(InstagramContext context) 
        {
            _context = context;
        }

        public async Task<bool> AddRefreshTokenAsync(AuthEntity authEntity)
        {
            // Mapea la entidad AuthEntity a RefreshTokenHistory y la almacena en la variable tokensToDb.
            var tokensToDb = AuthMapper.MapAuthEntityToRefreshToken(authEntity);

            // Agrega la entidad tokensToDb a la tabla Refreshtokenhistories de forma asincrónica.
            await _context.RefreshTokens.AddAsync(tokensToDb);

            // Intenta guardar los cambios en la base de datos de forma asincrónica y devuelve true si la operación tiene éxito.
            // Esta operación puede lanzar excepciones en caso de error.
            var saveChangesResult = await _context.SaveChangesAsync();

            // Si el resultado de guardar los cambios es mayor que cero, se considera una operación exitosa.
            return saveChangesResult > 0;
        }

        public async Task<bool> DeleteRefreshTokenAsync(AuthEntity authEntity)
        {
            // Buscar el token de refresco en la base de datos según el valor y el ID del usuario.
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(token => token.UserId == authEntity.UserId
                && token.TokenValue == authEntity.RefreshToken);

            // Si se encontró el token de refresco, elimínalo.
            if (refreshToken != null)
            {
                _context.RefreshTokens.Remove(refreshToken);
                var saveChangesResult = await _context.SaveChangesAsync();
                return saveChangesResult > 0;
            }

            // Si el token de refresco no se encontró, la eliminación no es exitosa.
            return false;
        }

        public async Task<Guid> FindUserIdAsync(AuthEntity authEntity)
        {
            var refreshToken = await _context.RefreshTokens
            .Where(token => token.UserId == authEntity.UserId && token.TokenValue == authEntity.RefreshToken)
            .FirstOrDefaultAsync();

            if (refreshToken is not null) return refreshToken.UserId;

            return Guid.Empty;
        }

        public async Task<bool> UpdateRefreshTokenAsync(AuthEntity authEntity)
        {
            // Buscar el token de refresco en la base de datos.
            // Esto asume que hay una entidad RefreshToken en tu contexto.
            var refreshToken = await _context.RefreshTokens
                .Where(token => token.UserId == authEntity.UserId && token.TokenValue == authEntity.RefreshToken)
                .FirstOrDefaultAsync();

            // Si el token de refresco se encontró en la base de datos, actualízalo.
            if (refreshToken != null)
            {
                // Actualiza el token de refresco con el nuevo valor, fecha de vencimiento, etc.
                refreshToken.TokenValue = authEntity.RefreshToken;

                // Guardar los cambios en la base de datos.
                var saveChangesResult = await _context.SaveChangesAsync();
                return saveChangesResult > 0;
            }

            // Si el token de refresco no se encontró, la actualización no es exitosa.
            return false;
        }
    }
}
