using System.Net;

namespace Instagram.App.UseCases.MediaCases.Types.Shared.Media.Events
{
    public class LikeEventType
    {
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string SenderUsername { get; set; } = null!;
        public string Message { get; set; } = null!;
        public HttpStatusCode StatusCode { get; set; }

    }
}
