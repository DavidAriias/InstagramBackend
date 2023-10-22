using Instagram.config.constants;
using Instagram.Domain.Repositories.Interfaces.Cache;
using Instagram.Infraestructure.Persistence.Context;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Instagram.Infraestructure.Data.Repositories.Cache
{
    public class RedisRepository : IRedisRepository
    {
        private readonly RedisContext _redisContext;
        public RedisRepository(RedisContext redisContext)
        {
            _redisContext = redisContext;
        }
        public async Task<string?> GetAsync(string key)
        {
            var result = await _redisContext.GetDatabase().StringGetAsync(key);

            if (!result.IsNull)
            {
                return result.ToString();
            }

            return null;
        }

        public async Task SetAsync(string key, string value, TimeSpan? expiration = null)
        {
            await _redisContext.GetDatabase().StringSetAsync(key, value, expiration);
        }
    }
}
