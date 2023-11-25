namespace Instagram.App.DTOs.User
{
    public class UnfollowUserDto
    {
        public Guid UserId { get; set; }
        public Guid FollowerId { get; set; }
    }
}
