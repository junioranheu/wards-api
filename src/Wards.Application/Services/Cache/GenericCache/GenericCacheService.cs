using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Wards.Application.Services.Cache.GenericCache;

public class GenericCacheService : IGenericCacheService
{
    private readonly IMemoryCache _memoryCache = memoryCache;

 /// <summary>
 /// Obter ou adicionar resultado em cache;
 /// Caso a key exista em cache, basicamente traga o resultado, senão, adicione-o em cache;
 /// var result = await _cache.GetOrAdd(_filters.Execute, SystemConsts.CacheKey_FiltersAtivo, TimeSpan.FromHours(24));
 /// </summary>
 public async Task<T?> GetOrAdd<T>(string key, Func<Task<T>> fetchFunction, TimeSpan expiration)
 {
     if (!_memoryCache.TryGetValue(key, out T? cacheEntry))
     {
         cacheEntry = await fetchFunction();
         _memoryCache.Set(key, cacheEntry, expiration);
     }

     return cacheEntry;
 }

 /// <summary>]
 /// Obter resultado em cache;
 /// var result = await _cache.Get<AtivoFiltersOutput>(SystemConsts.CacheKey_FiltersAtivo);
 /// </summary>
 public async Task<T?> Get<T>(string key)
 {
     _memoryCache.TryGetValue(key, out T? cacheEntry);
     return await Task.FromResult(cacheEntry);
 }

 /// <summary>
 /// Um método extensivo ao "GetOrAdd", mas, dessa vez, obtem ou adiciona resultado em cache em conjunto com um end-point que retorna um "Task<ActionResult>";
 /// var result = await _cache.GetObjectFromActionResult<AtivoFiltersOutput>(GetFilters, SystemConsts.CacheKey_FiltersAtivo);
 /// </summary>
 public async Task<T?> GetObjectFromActionResult<T>(Func<Task<ActionResult>> fetchFunction, string key) where T : class
 {
     var cachedObject = await Get<T>(key);

     if (cachedObject is null)
     {
         return await GetObject(fetchFunction);
     }

     if (cachedObject is T cacheResponse)
     {
         if (cacheResponse is null)
         {
             return await GetObject(fetchFunction);
         }
     }

     if (cachedObject is List<T> listCacheResponse)
     {
         if (listCacheResponse is null || listCacheResponse?.Count < 1)
         {
             return await GetObject(fetchFunction);
         }
     }

     return cachedObject;

     static async Task<T?> GetObject(Func<Task<ActionResult>> fetchFunction)
     {
         ActionResult actionResult = await fetchFunction();
         OkObjectResult? okResult = actionResult as OkObjectResult;

         if (okResult?.Value is T result)
         {
             return result;
         }

         return null;
     }
 }

 /// <summary>
 /// Limpar resultado em cache por uma ou mais keys;
 /// _cache.Clear([SystemConsts.CacheKey_FiltersAtivo, SystemConsts.CacheKey_FiltersPriorizacao]);
 /// </summary>
 public void Clear(string[] keys)
 {
     foreach (var key in keys)
     {
         _memoryCache.Remove(key);
     }
 }
}
