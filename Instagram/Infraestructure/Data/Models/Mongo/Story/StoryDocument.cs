using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Instagram.Infraestructure.Data.Models.Mongo.Shared.Media;

namespace Instagram.Infraestructure.Data.Models.Mongo.Story
{
    public class StoryDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("publish_date")]
        public DateTime PublishDate { get; set; }

        [BsonElement("user_id")]
        public string UserId { get; set; } = null!;

        [BsonElement("media_url")]
        public string Url { get; set; } = null!;

        [BsonElement("location")]
        public LocationDocument? Location { get; set; }

        [BsonElement("music")]
        public SongDocument? Song { get; set; }

    }
}
