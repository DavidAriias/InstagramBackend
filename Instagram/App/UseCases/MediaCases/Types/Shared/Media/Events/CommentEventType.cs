using System.Net;

namespace Instagram.App.UseCases.MediaCases.Types.Shared.Media.Events
{
    public class CommentEventType
    {
        public string CommentId { get; set; } = null!;
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string SenderUsername { get; set; }
        public string SenderMessage { get; set; } = null!;
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; } = null!;

        public CommentEventType(
            string commentId,
            Guid senderId,
            Guid receiverId,
            string senderUsername,
            string senderMessage,
            HttpStatusCode statusCode,
            string message)
        {
            CommentId = commentId;
            SenderId = senderId;
            ReceiverId = receiverId;
            SenderUsername = senderUsername;
            SenderMessage = senderMessage;
            StatusCode = statusCode;
            Message = message;
        }

        // Constructor para una respuesta exitosa
        public static CommentEventType CreateSuccess(
            string commentId,
            Guid sender,
            Guid receiver,
            string senderUsername,
            string senderMessage,
            string message)
        {
            return new CommentEventType(
                commentId,
                sender,
                receiver,
                senderUsername,
                senderMessage,
                HttpStatusCode.OK,
                message);
        }

        // Constructor para un mensaje de error
        public static CommentEventType CreateError(string errorMessage, HttpStatusCode statusCode)
        {
            return new CommentEventType(
                "",
                Guid.Empty, 
                Guid.Empty, 
                "", 
                "", 
                statusCode, 
                errorMessage);
        }
    }

}
