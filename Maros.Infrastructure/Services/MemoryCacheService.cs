using System.Collections.Concurrent;
using Maros.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Maros.Infrastructure.Services;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly ConcurrentDictionary<string, byte> _trackedKeys = new(StringComparer.OrdinalIgnoreCase);

    public MemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
    {
        if (_cache.TryGetValue(key, out T? cachedValue) && cachedValue is not null)
        {
            return cachedValue;
        }

        var value = await factory();
        if (value is not null)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5)
            };

            options.RegisterPostEvictionCallback((evictedKey, _, _, _) =>
            {
                _trackedKeys.TryRemove(evictedKey.ToString() ?? string.Empty, out _);
            });

            _cache.Set(key, value, options);
            _trackedKeys.TryAdd(key, 0);
        }

        return value;
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
        _trackedKeys.TryRemove(key, out _);
    }

    public void RemoveByPrefix(string prefix)
    {
        var keysToRemove = _trackedKeys.Keys
            .Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var key in keysToRemove)
        {
            _cache.Remove(key);
            _trackedKeys.TryRemove(key, out _);
        }
    }
}

