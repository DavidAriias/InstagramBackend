namespace Instagram.Domain.Entities.Auth
{
    public class AuthEntity
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public Guid UserId { get; set; }
     
    }
}
