using Instagram.App.UseCases.MediaCases.Types.Shared.Media;
using Instagram.App.UseCases.MusicCase;
using System.Net;

namespace Instagram.App.UseCases.MediaCases.Types.Reels
{
    public class ReelEventType
    {
        public Guid UserId { get; set; }
        public string? Caption { get; set; }
        public DateTime DatePublication { get; set; }
        public string Url { get; set; } = null!;
        public LocationType? LocationReel { get; set; }
        public List<string>? Tags { get; set; }
        public double Duration { get; set; }
        public SongMediaType? Song { get; set; }
        public List<MentionType>? Mentions { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; } = null!;
        public ReelEventType(
            string message,
            HttpStatusCode statusCode, 
            string? caption, 
            DateTime datePublication, 
            double duration,
            LocationType? locationReel,
            List<MentionType>? mentions, 
            SongMediaType? song, 
            List<string>? tags, 
            string url, 
            Guid userId)
        {
            Message = message;
            StatusCode = statusCode;
            Caption = caption;
            DatePublication = datePublication;
            Duration = duration;
            LocationReel = locationReel;
            Mentions = mentions;
            Song = song;
            Tags = tags;
            Url = url;
            UserId = userId;
        }


        public static ReelEventType CreateSuccess(
            string message,
            string? caption,
            DateTime datePublication,
            double duration,
            LocationType? locationReel,
            List<MentionType>? mentions,
            SongMediaType? song,
            List<string>? tags,
            string url,
            Guid userId)
        {
           
            return new ReelEventType(message, HttpStatusCode.OK, caption, datePublication, duration, locationReel, mentions, song, tags, url, userId);
        }


        // Método estático para crear una instancia de ReelEventType en caso de error.
        public static ReelEventType CreateError(HttpStatusCode statusCode, Guid userId, string errorMessage)
        {
            return new ReelEventType(
                errorMessage, 
                statusCode,
                "", 
                DateTime.MinValue,
                0, null,
                new List<MentionType>(), 
                null, 
                new List<string>(), 
                "", 
                userId);
        }

    }
}
