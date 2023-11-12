namespace Instagram.App.Auth
{
    public class AuthTypeIn
    {
        public Guid UserId { get; set; }
        public string? Token { get; set; }
        public string RefreshToken { get; set; } = null!;
    }
}
