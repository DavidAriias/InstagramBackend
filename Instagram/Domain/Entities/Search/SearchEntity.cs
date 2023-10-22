namespace Instagram.Domain.Entities.Search
{
    public class SearchEntity
    {
        public string Username { get; set; } = null!;
        public string? ImageProfile { get; set; }
        public string Name { get; set; } = null!;
        public Guid UserId { get; set; }
    }
}
