using Instagram.Domain.Entities.Auth;

namespace Instagram.Domain.Repositories.Interfaces.SQL.Token
{
    public interface ITokenSQLDbRepository
    {
        public Task<string?> FindUserIdAsync(Guid userId);
        public Task<Guid> FindRefreshTokenAsync(AuthEntity authEntity);
        public Task<bool> AddRefreshTokenAsync(AuthEntity authEntity);
        public Task<bool> UpdateRefreshTokenAsync(AuthEntity authEntity);
        public Task<bool> DeleteRefreshTokenAsync(AuthEntity authEntity);
    }
}
