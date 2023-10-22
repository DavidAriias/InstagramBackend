namespace Instagram.Domain.Repositories.Interfaces.Cache
{
    public interface IRedisRepository
    {
        public Task<string?> GetAsync(string key);
        public Task SetAsync(string key, string value, TimeSpan? expiration = null);
    }
}
