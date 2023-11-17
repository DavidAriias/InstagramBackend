using Instagram.Domain.Entities.Media;

namespace Instagram.Domain.Entities.User
{
    public class UserEntity
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string Password { get; set; } = null!;
        public string? Name { get; set; }
        public bool Isverificated { get; set; }
        public bool IsPrivate { get; set; }
        public string? Imageprofile { get; set; }
        public string? Description { get; set; }
        public string? UserPronoun { get; set; }
        public LinkEntity? Link { get; set; }
        public DateOnly UserBirthday { get; set; }
        public long PostsNumber { get; set; }
        public long FollowersCount { get; set; }
        public long FollowingCount { get; set; }
        public IEnumerable<PostEntity>? Posts { get; set; }
        public IEnumerable<ReelEntity>? Reels { get; set; }
        public IEnumerable<StoryEntity>? Stories { get; set; }
    }
}
