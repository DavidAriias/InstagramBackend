using Instagram.App.UseCases.MediaCases.Types.Shared.Media;
using Instagram.App.UseCases.MusicCase;
using System.Net;

namespace Instagram.App.UseCases.MediaCases.Types.Posts
{
    public class PostEventType
    {
        public Guid UserId { get; set; }
        public List<string> Images { get; set; } = null!;
        public string? Caption { get; set; }
        public DateTime DatePublication { get; set; }
        public List<string>? Hashtags { get; set; }
        public LocationType? LocationPost { get; set; }
        public SongMediaType? Song { get; set; }
        public List<MentionType>? Mentions { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; } = null!;

        public PostEventType(
            DateTime datePublication,
            List<string> images, 
            string? caption, 
            SongMediaType? song, 
            List<string>? hashtags,
            LocationType? locationPost,
            List<MentionType>? mentions, 
            Guid userId)
        {
            DatePublication = datePublication;
            Images = images;
            Caption = caption;
            Song = song;
            Hashtags = hashtags;
            LocationPost = locationPost;
            Mentions = mentions;
            UserId = userId;
        }

        public static PostEventType Create(
            DateTime datePublication,
            List<string> images,
            string? caption,
            SongMediaType? song,
            List<string>? hashtags,
            LocationType? locationPost,
            List<MentionType>? mentions,
            Guid userId)
        {
            return new PostEventType(datePublication, images, caption, song, hashtags, locationPost, mentions, userId);
        }

        public static PostEventType CreateError(HttpStatusCode statusCode, string errorMessage)
        {
            return new PostEventType(
                DateTime.MinValue, 
                new List<string>(), "", 
                null,
                new List<string>(),
                null, 
                new List<MentionType>(),
                Guid.Empty)
            {
                StatusCode = statusCode,
                Message = errorMessage
            };
        }

    }
}
