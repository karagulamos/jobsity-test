using System.Text.Json;
using Jobsity.Chat.Core.Persistence;
using Microsoft.Extensions.Caching.Distributed;

namespace Jobsity.Chat.Persistence.DistributedCache;

public class DistributedMemoryCache : ICache
{
    private readonly IDistributedCache _cache;

    public DistributedMemoryCache(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T> GetAsync<T>(string key)
    {
        var value = await _cache.GetStringAsync(key);

        if (string.IsNullOrEmpty(value))
            return default!;

        return JsonSerializer.Deserialize<T>(value) ?? default!;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = default)
    {
        var options = new DistributedCacheEntryOptions();

        if (expiry.HasValue)
            options.SetAbsoluteExpiration(expiry.Value);

        var cacheValue = JsonSerializer.Serialize(value);

        await _cache.SetStringAsync(key, cacheValue, options);
    }
}