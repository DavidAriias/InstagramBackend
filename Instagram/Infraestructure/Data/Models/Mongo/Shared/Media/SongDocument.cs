using MongoDB.Bson.Serialization.Attributes;

namespace Instagram.Infraestructure.Data.Models.Mongo.Shared.Media
{
    public class SongDocument
    {
        public string Title { get; set; } = null!;
        public string Artist { get; set; } = null!;
        public string Url { get; set; } = null!;

        [BsonElement("total_duration")]
        public string TotalDuration { get; set; } = null!;

        [BsonElement("selection")]
        public DurationSelectionDocument Selection { get; set; } = null!;
    }
}
