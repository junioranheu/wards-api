namespace Wards.Application.Services.Cache.GenericCache;

public interface IGenericCacheService
{
    void Clear(string[] keys);
    Task<T?> GetOrAdd<T>(string key, Func<Task<T>> fetchFunction, TimeSpan expiration);
}