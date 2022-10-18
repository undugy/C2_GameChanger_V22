using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.StackExchangeRedis;
namespace Server.Services;

public class MemcacheManager
{
    private static MemoryCache _cache;
    public static void Init()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddMemoryCache();
        IServiceProvider serviceProvider = services.BuildServiceProvider();
        _cache=serviceProvider.GetService<IMemoryCache>() as MemoryCache;
        //var connection
        _cache = new MemoryCache(new MemoryCacheOptions());
    }

 
    public static MemoryCache GetCacheConnection()
    {
        return _cache;
    }
    
    private static void AfterEvicted(object key, object value, EvictionReason reason, object state)
    {
        Console.WriteLine("Evicted. Value: " + value + ", Reason: " + reason);
    }

    public static T Get<T>(string key)  
    {
        T value;
        if (false == _cache.TryGetValue<T>(key, out value))
        {
            return default(T);
        }
        return value;
    }

    public static void Set<T>(string key, T value)
    {
        _cache.Set(key, value);
        
    }
    
}