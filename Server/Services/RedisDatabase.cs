using CloudStructures;
using CloudStructures.Structures;
using Server.Interface;
using Dapper;
using Server.Table;
using ZLogger;

namespace Server.Services;
public class RedisDatabase:IRedisDatabase
{
    private RedisConnection _redisConn;

    public RedisConnection GetConnection() => _redisConn;
    private static RedisConfig _config;
    private readonly ILogger _logger;
    public static void Init(IConfiguration configuration)
    {
        _config = new RedisConfig("basic", configuration.GetSection("DBConnection")["Redis"]);
        
    }

    private async Task<ErrorCode> SetMasterTable<TKey, TVal>(string key, IEnumerable<KeyValuePair<TKey, TVal>> table)where TKey:notnull
    {
        var redisDict = GetHash<TKey, TVal>(key);
        await redisDict.SetAsync(table);
        return ErrorCode.NONE;
    }

    public RedisDatabase(ILogger<RedisDatabase>logger)
    {
        _redisConn = new RedisConnection(_config);
        _logger = logger;
    }
    
    
    public async Task<T>GetHashValue<TKey,T>(string key,TKey subKey)where TKey:notnull
    {
        var redisId = new RedisDictionary<TKey,T>(GetConnection(),key,null);
        
        var res= await redisId.GetAsync(subKey);
        return res.Value;
    }
    
    public  RedisDictionary<TKey,TVal>GetHash<TKey,TVal>(string key)where TKey:notnull
    {
        var redisId = new RedisDictionary<TKey,TVal>(GetConnection(),key,null);
        
        return redisId;
    }
    
    public  async Task<T> GetStringValue<T>(string key)
    {
        var redisId = new RedisString<T>(GetConnection(),key,null);
        var result = await redisId.GetAsync();
        return result.Value;
    }
    public  async Task<bool> SetStringValue<T>(string key, T value)
    {
        var defaultExpiry = TimeSpan.FromDays(1);
        var redisId = new RedisString<T>(GetConnection(),key,defaultExpiry);
        return await redisId.SetAsync(value);
    }
    public async Task<bool>SetHashValue<TKEY,T>(string key,TKEY subKey,T value)
        where T:class 
        where TKEY:notnull
    {
        var defaultExpiry = TimeSpan.FromDays(1);
        var redisId = new RedisDictionary<TKEY,T>(GetConnection(),key,defaultExpiry);
        var result= await redisId.SetAsync(subKey, value);
        return result;
    }
    
    
    
}

