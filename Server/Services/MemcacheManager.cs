using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.StackExchangeRedis;
namespace Server.Services;

public class MemcacheManager
{
    private const string config = "localhost";
    private const string instanceNmae = "sampleCahe";
    public static void Init()
    {
        
    }

 
    public static MemoryCache GetCacheConnection()
    {
        var connection = new MemoryCache(new MemoryCacheOptions());
        return connection;
    }
    
    private static void AfterEvicted(object key, object value, EvictionReason reason, object state)
    {
        Console.WriteLine("Evicted. Value: " + value + ", Reason: " + reason);
    }
    
}