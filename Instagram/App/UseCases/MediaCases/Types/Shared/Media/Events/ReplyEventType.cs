using System.Net;

namespace Instagram.App.UseCases.MediaCases.Types.Shared.Media.Events
{
    public class ReplyEventType
    {
        public string ReplyId { get; set; } = null!;
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string SenderUsername { get; set;} = null!;
        public string Message { get; set; } = null!;
        public HttpStatusCode StatusCode { get; set; }
        public string SenderMessage { get; set; } = null!;

        // Constructor vacío
        public ReplyEventType()
        {
        }

        // Constructor con parámetros
        public ReplyEventType(
            Guid senderId, 
            string senderMessage, 
            string senderUsername,
            string replyId,
            Guid receiverId, 
            string message, 
            HttpStatusCode statusCode)
        {
            SenderId = senderId;
            SenderMessage = senderMessage;
            SenderUsername = senderUsername;
            ReplyId = replyId;
            ReceiverId = receiverId;
            Message = message;
            StatusCode = statusCode;
        }

        // Constructor para mensajes de éxito
        public static ReplyEventType CreateSuccess(
            Guid senderId, 
            string senderMessage, 
            string senderUsername, 
            string replyId,
            Guid receiverId, 
            string message)
        {
            return new ReplyEventType(senderId, senderMessage, senderUsername, replyId, receiverId, message, HttpStatusCode.OK);
        }

        // Constructor para mensajes de error interno
        // Constructor estático para crear una respuesta de error de parámetro
        public static ReplyEventType CreateError(
            Guid senderId, 
            HttpStatusCode statusCode, 
            string message)
        {
            return new ReplyEventType(senderId, "", "", "", Guid.Empty, message, statusCode);
        }
    }
}
