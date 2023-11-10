namespace Instagram.Domain.Repositories.Interfaces.Auth
{
    public interface IJwtService
    {
        public string GenerateAccessToken(string userId);
        public string GenerateRefreshToken(string userId);
        public bool IsTokenValid(string token);
        public bool IsRefreshToken(string token);
    }
}
