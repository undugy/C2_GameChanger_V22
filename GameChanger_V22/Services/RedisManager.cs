using CloudStructures;
namespace GameChanger_V22.Services;
public class RedisManager
{
    public static RedisConnection s_redisConn { get; set; }

    public static void Init(String address)
    {
        
            var config = new RedisConfig("basic", address);
            s_redisConn = new RedisConnection(config);
        
    }
}