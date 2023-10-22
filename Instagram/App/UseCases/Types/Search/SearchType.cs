namespace Instagram.App.UseCases.Types.Search
{
    public class SearchType
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = null!;
        public string? Name { get; set; } 
        public string? ImageProfile { get; set; }
    }
}
