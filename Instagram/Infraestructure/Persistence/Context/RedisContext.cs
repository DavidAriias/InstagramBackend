using Instagram.config.constants;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Instagram.Infraestructure.Persistence.Context
{
    public class RedisContext
    {
        private readonly ConnectionMultiplexer _connectionMultiplexer;
        public RedisContext(IOptions<RedisConfig> redisConfig)
        {
            var config = redisConfig.Value;

            var configurationOptions = new ConfigurationOptions
            {
                EndPoints = { $"{config.Host}:{config.Port}" },
                Password = config.Password,
                AbortOnConnectFail = false
            };

            _connectionMultiplexer = ConnectionMultiplexer.Connect(configurationOptions);
        }

        public IDatabase GetDatabase() => _connectionMultiplexer.GetDatabase();

    }
}
