using Instagram.App.UseCases.UserCase.Types;
using Instagram.App.UseCases.UserCase.Types.Events;

namespace Instagram.App.UseCases.UserCase.Followed
{
    public interface IFollowedCase
    {
        public Task<FollowedEventType> FollowToUser(Guid followerId, Guid followedId);
        public Task<IEnumerable<FollowerType>?> GetUsersFollowedByOthers(Guid userId);
    }
}
