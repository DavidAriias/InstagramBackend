using Instagram.App.UseCases.MediaCases.Types.Shared.Media;
using Instagram.App.UseCases.MusicCase;
using System.Net;

namespace Instagram.App.UseCases.MediaCases.Types.Stories
{
    public class StoryEventType
    {
        public Guid UserId { get; set; }
        public DateTime PostDate { get; set; }
        public LocationType? LocationStory { get; set; }
        public long Duration { get; set; }
        public string MediaUrl { get; set; } = null!;
        public SongMediaType? SongMedia { get; set; }
        public string Message { get; set; } = null!;
        public HttpStatusCode StatusCode { get; set; }

        // Constructor estático para crear una respuesta exitosa
        public static StoryEventType CreateSuccessResponse(
            Guid userId,
            long duration,
            LocationType? locationStory,
            string mediaUrl,
            DateTime postDate,
            SongMediaType? songMedia)
        {
            return new StoryEventType
            {
                UserId = userId,
                Duration = duration,
                LocationStory = locationStory,
                MediaUrl = mediaUrl,
                PostDate = postDate,
                SongMedia = songMedia,
                StatusCode = HttpStatusCode.OK,
                Message = "The media has been uploaded successfully"
            };
        }

        // Constructor estático para crear una respuesta de error
        public static StoryEventType CreateErrorResponse()
        {
            return new StoryEventType
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "There was an error while processing the request"
            };
        }
    }
}
