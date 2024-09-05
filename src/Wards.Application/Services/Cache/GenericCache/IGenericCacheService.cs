using Microsoft.AspNetCore.Mvc;

namespace Wards.Application.Services.Cache.GenericCache;

public interface IGenericCacheService
{
    Task<T?> GetOrAdd<T>(Func<Task<T>> fetchFunction, string key, TimeSpan expiration);
    Task<T?> GetOrAddSimple<T>(Func<Task<T>> fetchFunction, string key, TimeSpan expiration);
    Task<T?> GetOrAddWithQueue<T>(Func<Task<T>> fetchFunction, string key, TimeSpan expiration);
    Task<T?> Get<T>(string key);
    Task<T?> GetObjectFromActionResult<T>(Func<Task<ActionResult>> fetchFunction, string key) where T : class;
    void Clear(string[] keys);
}
