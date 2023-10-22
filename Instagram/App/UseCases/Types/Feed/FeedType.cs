namespace Instagram.App.UseCases.Types.Feed
{
    public class FeedType<T>
    {
        public string Username { get; set; } = null!;
        public string? ImageProfile { get; set; }

        [UsePaging]
        public IEnumerable<T>? Media { get; set; }

    }
}
