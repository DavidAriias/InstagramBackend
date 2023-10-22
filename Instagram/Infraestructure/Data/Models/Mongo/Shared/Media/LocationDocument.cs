using MongoDB.Bson.Serialization.Attributes;

namespace Instagram.Infraestructure.Data.Models.Mongo.Shared.Media
{
    public class LocationDocument
    {

        [BsonElement("latitude")]
        public double Latitude { get; set; }

        [BsonElement("longitude")]
        public double Longitude { get; set; }
    }
}








