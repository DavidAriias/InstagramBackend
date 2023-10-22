using Instagram.App.UseCases.Types.Search;
using Instagram.Domain.Entities.Search;
using Instagram.Infraestructure.Data.Models.SQL;

namespace Instagram.Infraestructure.Mappers.Search
{
    public static class SearchMapper
    {
        public static SearchType MapSearchEntityToSearchType(SearchEntity searchEntity) => new()
        {
            ImageProfile = searchEntity.ImageProfile,
            Name = searchEntity.Name,
            UserId = searchEntity.UserId,
            Username = searchEntity.Username
        };

        public static SearchEntity MapSearchStoredProcedureToSearchEntity(SearchStoredProcedure storedProcedure) => new()
        {
            Username = storedProcedure.Username,
            ImageProfile = storedProcedure.ImageProfile,
            Name = storedProcedure.Name,
            UserId = storedProcedure.UserId
        };
    }
}
