using Instagram.App.UseCases.Types.Shared;
using Instagram.App.UseCases.UserCase.Types;

namespace Instagram.App.UseCases.UserCase.Followers
{
    public interface IFollowerCase
    {
        public Task<IEnumerable<FollowerType>?> GetFollowersFromUser(Guid userId);
        public  Task<bool> IsUserFollowToOther(Guid followerId, Guid userId);
        public Task<ResponseType<Guid>> UnfollowUser(Guid followerId, Guid userId);
    }
}
