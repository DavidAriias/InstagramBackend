using Instagram.App.UseCases.Types.Search;

namespace Instagram.App.UseCases.SearchCase
{
    public interface ISeachCase
    {
        public Task<IReadOnlyCollection<SearchType>?> SearchUsers(string input);
    }
}
