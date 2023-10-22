using Instagram.App.UseCases.UserCase.Types;
using Instagram.Domain.Entities;

namespace Instagram.App.UseCases.UserCase.SuggestFollowers
{
    public interface ISuggestFollowersCase
    {
        public Task<IEnumerable<FollowerType>> GetSuggestionFollowers(Guid userId);
    }
}
