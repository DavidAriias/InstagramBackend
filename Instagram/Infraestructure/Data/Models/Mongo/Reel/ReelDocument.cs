using Instagram.Infraestructure.Data.Models.Mongo.Shared.Media;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Instagram.Infraestructure.Data.Models.Mongo.Reel
{
    public class ReelDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("publish_date")]
        public DateTime PublishDate { get; set; }

        [BsonElement("user_id")]
        public string UserId { get; set; } = null!;

        [BsonElement("video_url")]
        public string Url { get; set; } = null!;

        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("mentions")]
        public List<MentionDocument>? Mentions { get; set; }

        [BsonElement("location")]
        public LocationDocument? Location { get; set; }

        [BsonElement("likes")]
        public long Likes { get; set; }

        [BsonElement("comments")]
        public List<CommentDocument>? Comments { get; set; }

        [BsonElement("duration")]
        public double Duration { get; set; }

        [BsonElement("hashtags")]
        public List<string>? Hashtags { get; set; }

        [BsonElement("music")]
        public SongDocument? Song { get; set; }
    }
}
