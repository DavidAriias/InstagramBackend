namespace Instagram.App.DTOs.Media
{
    public class DeleteReelDto
    {
        public string MediaId { get; set; } = null!;
        public Guid UserId { get; set; }
    }
}
