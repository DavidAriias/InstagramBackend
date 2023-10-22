using Instagram.Domain.Entities.Search;

namespace Instagram.Domain.Repositories.Interfaces.SQL.Search
{
    public interface ISearchSQLDbRepository
    {
        public Task<List<SearchEntity>?> SearUsersByAnInput(string searchTerm);
    }
}
