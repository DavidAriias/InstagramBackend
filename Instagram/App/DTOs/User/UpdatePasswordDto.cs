namespace Instagram.App.DTOs.User
{
    public class UpdatePasswordDto
    {
        public string Password { get; set; } = null!;
        public Guid UserId { get; set; }
    }
}
