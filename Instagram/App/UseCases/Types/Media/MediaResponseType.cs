namespace Instagram.App.UseCases.Types.Media
{
    public class MediaResponseType<T>
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = null!;
        public string? ImageProfile { get; set; }
        public IEnumerable<T>? Media { get; set; }
    }
}
