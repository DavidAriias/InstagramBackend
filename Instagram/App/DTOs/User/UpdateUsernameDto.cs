namespace Instagram.App.DTOs.User
{
    public class UpdateUsernameDto
    {
        public string Username { get; set; } = null!;
        public Guid UserId { get; set; }
    }
}
