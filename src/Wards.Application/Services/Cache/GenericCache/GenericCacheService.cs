using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using static Wards.Utils.Fixtures.Get;

namespace Wards.Application.Services.Cache.GenericCache;

public class GenericCacheService : IGenericCacheService
{
    private readonly IMemoryCache _memoryCache;
    private static readonly SemaphoreSlim semaphore = new(1, 1);

    public GenericCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    /// <summary>
    /// Obter ou adicionar resultado em cache (de uma forma mais robusta e segura, utilizando Lazy<Task<T>> e Try Catch);
    /// Caso a key exista em cache, basicamente traga o resultado, senão, adicione-o em cache;
    /// var result = await _cache.GetOrAdd(_filters.Execute, SystemConsts.CacheKey_FiltersAtivo, TimeSpan.FromHours(24));
    /// </summary>
    public async Task<T?> GetOrAdd<T>(Func<Task<T>> fetchFunction, string key, TimeSpan expiration)
    {
        try
        {
            if (!_memoryCache.TryGetValue(key, out Lazy<Task<T>>? lazyCacheEntry))
            {
                Lazy<Task<T>> lazyValue = new(() => fetchFunction());
                _memoryCache.Set(key, lazyValue, expiration);
                lazyCacheEntry = lazyValue;
            }

            T? cachedValue = await lazyCacheEntry!.Value ?? default;

            if (cachedValue is null || IsEmptyList(cachedValue))
            {
                _memoryCache.Remove(key);
                return default;
            }

            return cachedValue;
        }
        catch
        {
            _memoryCache.Remove(key);
            throw;
        }
    }

    /// <summary>
    /// Obter ou adicionar resultado em cache (da forma mais simples possível, sem utilizar Lazy ou Try Catch);
    /// </summary>
    public async Task<T?> GetOrAddSimple<T>(Func<Task<T>> fetchFunction, string key, TimeSpan expiration)
    {
        if (!_memoryCache.TryGetValue(key, out T? cacheEntry))
        {
            cacheEntry = await fetchFunction();
            _memoryCache.Set(key, cacheEntry, expiration);
        }

        return cacheEntry;
    }

    /// <summary>
    /// Obter ou adicionar resultado em cache (utilizando fila (SemaphoreSlim) para assegurar-se que apenas um método é executado por vez, evitando duplicidade e gasto de recursos à toa);
    /// </summary>
    public async Task<T?> GetOrAddWithQueue<T>(Func<Task<T>> fetchFunction, string key, TimeSpan expiration)
    {
        await semaphore.WaitAsync();

        try
        {
            T? cachedValue = await GetOrAdd(fetchFunction, key, expiration);
            return cachedValue;
        }
        finally
        {
            semaphore.Release();
        }
    }

    /// <summary>
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

        if (cachedObject is null || IsEmptyList(cachedObject))
        {
            return await GetObject(fetchFunction);
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