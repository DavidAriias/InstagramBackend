namespace Instagram.Domain.Repositories.Interfaces.Auth
{
    public interface IJwtService
    {
        public string GenerateAccessToken(string userId);
        public string GenerateRefreshToken(string userId);
        public Task<bool> IsValidateToken(string token);
    }
}
