namespace Instagram.App.DTOs.User
{
    public class FollowToUserDto
    {
        public Guid FollowerId { get; set; }
        public Guid FollowedUserId { get; set; }
    }
}
