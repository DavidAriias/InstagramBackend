using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Instagram.Infraestructure.Data.Models.Mongo.Shared.Media
{
    public class CommentDocument
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId CommentId { get; set; } = ObjectId.GenerateNewId();

        [BsonElement("user_id")]
        public string UserId { get; set; } = null!;

        [BsonElement("text")]
        public string Text { get; set; } = null!;

        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("replies")]
        public List<ReplyDocument>? Replies { get; set; }

    }
}
