using Microsoft.Extensions.Caching.Memory;

namespace Pim.Utility
{
    public class CacheService
    {
        private readonly IMemoryCache _cache;

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<T> GetOrSetAsync<T>(
            string key,
            Func<Task<T>> getData,
            TimeSpan? expiration = null)
        {
            if (_cache.TryGetValue(key, out T cachedValue))
            {
                return cachedValue;
            }

            T value = await getData();

            var options = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(expiration ?? TimeSpan.FromMinutes(5)).SetSlidingExpiration(TimeSpan.FromMinutes(2));

            if (value != null)
            {
                _cache.Set(key, value, options);
            }

            return value;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

    }
}
