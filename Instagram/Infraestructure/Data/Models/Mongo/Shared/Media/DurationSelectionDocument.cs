using MongoDB.Bson.Serialization.Attributes;

namespace Instagram.Infraestructure.Data.Models.Mongo.Shared.Media
{
    public class DurationSelectionDocument
    {
        [BsonElement("start")]
        public string Start { get; set; } = null!;

        [BsonElement("end")]
        public string End { get; set; } = null!;
    }
}
