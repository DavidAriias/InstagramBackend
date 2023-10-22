using Instagram.App.UseCases.MediaCases.Types.Shared.Media;
using Instagram.App.UseCases.Types.Shared;
using Instagram.Domain.Entities.Shared.Media;
using Instagram.Domain.Enums;
using Instagram.Domain.Repositories.Interfaces.Document.Post;
using Instagram.Domain.Repositories.Interfaces.Document.Reel;
using Instagram.Infraestructure.Mappers.Shared.Media;

namespace Instagram.App.UseCases.MediaCases.LocationCase
{
    public class LocationCase : ILocationCase
    {
        private readonly IPostMongoDbRepository _postRepository;
        private readonly IReelMongoDbRepository _reelRepository;
        public LocationCase(IPostMongoDbRepository postRepository, IReelMongoDbRepository reelRepository) 
        {
            _postRepository = postRepository;
            _reelRepository = reelRepository;
        }

        public async Task<ResponseType<string>> UpdateLocation(LocationType locationType, string mediaId, ContentEnum contentType)
        {
            bool isUpdated = false;
            LocationEntity? locationEntity;
            switch (contentType)
            {
                case ContentEnum.Post:
                    locationEntity = MediaMapper.MapLocationTypeToLocationEntity(locationType);
                    isUpdated = await _postRepository.EditLocationAsync(locationEntity, mediaId);
                    break;
                case ContentEnum.Reel:
                    locationEntity = MediaMapper.MapLocationTypeToLocationEntity(locationType);
                    isUpdated = await _reelRepository.EditLocationAsync(locationEntity, mediaId);
                    break;
            }

            ResponseType<string> response;

            if (isUpdated)
            {
                response = ResponseType<string>.CreateSuccessResponse("Location updated successfully.");
            }
            else
            {
                response = ResponseType<string>.CreateErrorResponse("Failed to update location.",
                    System.Net.HttpStatusCode.InternalServerError);
            }

            return response;
        }

    }
}

