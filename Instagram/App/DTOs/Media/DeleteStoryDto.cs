namespace Instagram.App.DTOs.Media
{
    public class DeleteStoryDto
    {
        public string MediaId { get; set; } = null!;
        public Guid UserId { get; set; }
    }
}
