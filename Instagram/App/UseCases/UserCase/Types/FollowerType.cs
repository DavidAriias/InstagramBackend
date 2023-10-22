namespace Instagram.App.UseCases.UserCase.Types
{
    public class FollowerType
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string? ImageProfile { get; set; }

        public FollowerType(Guid id, string? imageProfile, string username)
        {
            Id = id;
            ImageProfile = imageProfile;
            Username = username;
        }

        public static FollowerType Create(Guid id, string? imageProfile, string username)
        {
            return new FollowerType(id, imageProfile, username);
        }

    }
}
