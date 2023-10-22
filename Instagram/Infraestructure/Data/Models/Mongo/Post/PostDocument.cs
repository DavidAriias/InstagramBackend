using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Instagram.Infraestructure.Data.Models.Mongo.Shared.Media;

namespace Instagram.Infraestructure.Data.Models.Mongo.Post
{
    public class PostDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("user_id")]
        public string UserId { get; set; } = null!;

        [BsonElement("image")]
        public List<string> Images { get; set; } = null!;

        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("publish_date")]
        public DateTime PublishDate { get; set; }

        [BsonElement("likes")]
        public long Likes { get; set; }

        [BsonElement("comments")]
        public List<CommentDocument>? Comments { get; set; }

        [BsonElement("hashtags")]
        public List<string>? Hashtags { get; set; }

        [BsonElement("location")]
        public LocationDocument? Location { get; set; }

        [BsonElement("music")]
        public SongDocument? Song { get; set; }

        [BsonElement("mentions")]
        public List<MentionDocument>? Mentions { get; set; }
    }
}
