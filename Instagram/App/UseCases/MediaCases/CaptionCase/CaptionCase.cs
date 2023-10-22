using Instagram.App.UseCases.MediaCases.Types.Shared.Media;
using Instagram.App.UseCases.Types.Shared;
using Instagram.Domain.Enums;
using Instagram.Domain.Repositories.Interfaces.Document.Post;
using Instagram.Domain.Repositories.Interfaces.Document.Reel;

namespace Instagram.App.UseCases.MediaCases.CaptionCase
{
    public class CaptionCase : ICaptionCase
    {
        private readonly IPostMongoDbRepository _postRepository;
        private readonly IReelMongoDbRepository _reelNeo4jRepository;
        public CaptionCase(IPostMongoDbRepository postRepository, 
            IReelMongoDbRepository reelNeo4jRepository) 
        {
            _postRepository = postRepository;
            _reelNeo4jRepository = reelNeo4jRepository;
        }

        public async Task<ResponseType<string>> UpdateCaption(CaptionTypeIn captionType)
        {
            bool isUpdated = false;

            switch (captionType.ContentType)
            {
                case ContentEnum.Post:
                    isUpdated = await _postRepository.EditCaptionAsync(captionType.Caption, captionType.MediaId);
                    break;
                case ContentEnum.Reel:
                    isUpdated = await _reelNeo4jRepository.EditCaptionAsync(captionType.Caption, captionType.MediaId);
                    break;
            }

            ResponseType<string> response;

            if (isUpdated)
            {
                response = ResponseType<string>.CreateSuccessResponse("Caption updated successfully.");
            }
            else
            {
                response = ResponseType<string>.CreateErrorResponse("Failed to update caption.",
                    System.Net.HttpStatusCode.InternalServerError);
            }

            return response;
        }

    }
}
