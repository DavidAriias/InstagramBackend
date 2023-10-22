using Instagram.App.UseCases.MediaCases.Types.Shared.Media;
using Instagram.App.UseCases.Types.Shared;
using Instagram.Domain.Enums;

namespace Instagram.App.UseCases.MediaCases.LocationCase
{
    public interface ILocationCase
    {
        public Task<ResponseType<string>> UpdateLocation(LocationType locationType, string mediaId, ContentEnum contentType);
    }
}
