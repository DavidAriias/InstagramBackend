using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Instagram.Infraestructure.Data.Models.Mongo.Shared.Media
{
    public class ReplyDocument
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId ReplyId { get; set; } = ObjectId.GenerateNewId();

        [BsonElement("user_id")]
        public string UserId { get; set; } = null!;

        [BsonElement("text")]
        public string Text { get; set; } = null!;

        [BsonElement("date")]
        public DateTime Date { get; set; }
    }
}
