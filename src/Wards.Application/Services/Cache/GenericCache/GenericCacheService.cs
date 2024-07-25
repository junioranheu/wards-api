using Microsoft.Extensions.Caching.Memory;

namespace Wards.Application.Services.Cache.GenericCache;

public class GenericCacheService : IGenericCacheService
{
    private readonly IMemoryCache _memoryCache;

    public GenericCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public async Task<T?> GetOrAdd<T>(string key, Func<Task<T>> fetchFunction, TimeSpan expiration)
    {
        if (!_memoryCache.TryGetValue(key, out T? cacheEntry))
        {
            cacheEntry = await fetchFunction();
            _memoryCache.Set(key, cacheEntry, expiration);
        }

        return cacheEntry;
    }

    public void Clear(string[] keys)
    {
        foreach (var key in keys)
        {
            _memoryCache.Remove(key);
        }
    }
}