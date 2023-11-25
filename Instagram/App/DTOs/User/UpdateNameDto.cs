namespace Instagram.App.DTOs.User
{
    public class UpdateNameDto
    {
        public string Name { get; set; } = null!;
        public Guid UserId { get; set; }
    }
}
