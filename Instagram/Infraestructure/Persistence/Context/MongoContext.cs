using Instagram.config.constants;
using Instagram.Infraestructure.Data.Models.Mongo.Post;
using Instagram.Infraestructure.Data.Models.Mongo.Reel;
using Instagram.Infraestructure.Data.Models.Mongo.Story;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Instagram.Infraestructure.Persistence.Context
{
    public class MongoContext
    {
        private readonly IMongoDatabase _database;

        public MongoContext(IOptions<MongoConfig> mongoConfig)
        {
            var config = mongoConfig.Value;

            var client = new MongoClient(config.ConnectionString);
            _database = client.GetDatabase(config.DatabaseName);
        }

        public IMongoDatabase GetDatabase => _database;
        public IMongoCollection<StoryDocument> Stories => GetDatabase.GetCollection<StoryDocument>("stories");
        public IMongoCollection<PostDocument> Posts => GetDatabase.GetCollection<PostDocument>("posts");
        public IMongoCollection<ReelDocument> Reels => GetDatabase.GetCollection<ReelDocument>("reels");
    }
}
