namespace Instagram.App.UseCases.UserCase.Types.Events
{
    public class FollowedEventType
    {
        public Guid FollowerId { get; set; }
        public string FollowerName { get; set; } = null!;
        public string? FollowerImageProfile { get; set; }
        public Guid FollowedUserId { get; set; }

        public FollowedEventType
            (Guid followedUserId, 
            Guid followerId,
            string followerName,
            string? followerImageProfile)
        {
            FollowedUserId = followedUserId;
            FollowerId = followerId;
            FollowerName = followerName;
            FollowerImageProfile = followerImageProfile;
        }

        public FollowedEventType()
        {
            // Constructor sin argumentos
        }
        public static FollowedEventType Create(
            Guid followedUserId,
            Guid followerId, 
            string followerName, 
            string? followerImageProfile)
        {
            return new FollowedEventType(followedUserId, followerId, followerName, followerImageProfile);
        }
    }
}
