using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Instagram.Infraestructure.Data.Models.Mongo.Shared.Media
{
    public class MentionDocument
    {
        [BsonElement("user_id")]
        public string UserId { get; set; } = null!;

    }
}
