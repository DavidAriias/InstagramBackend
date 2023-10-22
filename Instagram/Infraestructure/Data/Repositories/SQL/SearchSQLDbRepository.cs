using Instagram.Domain.Entities.Search;
using Instagram.Domain.Repositories.Interfaces.SQL.Search;
using Instagram.Infraestructure.Mappers.Search;
using Instagram.Infraestructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Instagram.Infraestructure.Data.Repositories.SQL
{
    public class SearchSQLDbRepository : ISearchSQLDbRepository
    {
        private readonly InstagramContext _context;
        public SearchSQLDbRepository(InstagramContext context) 
        {
            _context = context;
        }

        public async Task<List<SearchEntity>?> SearUsersByAnInput(string searchTerm)
        {
            var searchStoredProcedures = await _context.SearchStoredProcedures
               .FromSqlRaw("SELECT * FROM get_users_with_search(@search_term)",
               new NpgsqlParameter("search_term", searchTerm))
               .ToListAsync();

           return searchStoredProcedures
                .Select(s => SearchMapper.MapSearchStoredProcedureToSearchEntity(s))
                .ToList();
        }

    }
}
