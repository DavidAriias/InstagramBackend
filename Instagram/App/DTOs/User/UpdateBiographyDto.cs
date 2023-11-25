namespace Instagram.App.DTOs.User
{
    public class UpdateBiographyDto
    {
        public string Biography { get; set; } = null!;
        public Guid UserId { get; set; }
    }
}
