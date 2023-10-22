namespace Instagram.Infraestructure.Data.Models.SQL
{
    public class SearchStoredProcedure
    {
        public string Username { get; set; } = null!;
        public string? ImageProfile { get; set; }
        public string? Name { get; set; }
        public Guid UserId { get; set; }
    }
}
