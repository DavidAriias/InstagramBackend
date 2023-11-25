namespace Instagram.App.DTOs.User
{
    public class UpdateImageProfileDto
    {
        public IFormFile Image { get; set; } = null!;
        public Guid UserId { get; set; }
    }
}
