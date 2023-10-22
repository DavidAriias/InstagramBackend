using Instagram.Domain.Entities.Media;
using Instagram.Domain.Entities.User;

namespace Instagram.Domain.Repositories.Interfaces.Graph.User
{
    public interface IUserNeo4jRepository
    {
        public Task CreateUser(UserEntity user);
        public Task UpdateUsername(string username, string userId);
        public Task FollowToUser(string followerId, string userId);
        public Task<IEnumerable<string>> FollowersFromUser(string userId);
        public Task<bool> IsUserFollowToOther(string followerId, string userId);
        public Task UnfollowUser(string followerId, string userId);
        public Task<IEnumerable<string>> UsersFollowedByOthers(string userId);
        public Task<IEnumerable<string>> SuggestionFollowers(string userId);
        public Task<IEnumerable<PostEntity>> SuggestionPosts(string userId);
        public Task<IEnumerable<ReelEntity>> SuggestionReels(string userId);

    }
}
