namespace Instagram.App.DTOs.User
{
    public class UpdateBirthdayDto
    {
        public DateOnly Birthday { get; set; }
        public Guid UserId { get; set; }
    }
}
