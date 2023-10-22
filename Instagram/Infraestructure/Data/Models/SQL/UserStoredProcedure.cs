namespace Instagram.Infraestructure.Data.Models.SQL
{
    public class UserStoredProcedure
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImageProfile { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsVerificated { get; set; }
        public string? LinkLink { get; set; }
        public string? LinkTitle { get; set; }
        public string? Name { get; set; }
        public string? UserPronoun { get; set; }
        public DateOnly UserBirthday { get; set; }
    }

}
