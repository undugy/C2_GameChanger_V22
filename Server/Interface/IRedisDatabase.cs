using CloudStructures;
using CloudStructures.Structures;

namespace Server.Interface;

public interface IRedisDatabase
{
    public RedisConnection GetConnection();
    public Task<T> GetHashValue<Tkey,T>(string key, Tkey subKey)where Tkey:notnull;
    public Task<List<T>> GetListByRange<T>(string key);
    public RedisDictionary<T_KEY, T> GetHash<T_KEY, T>(string key)where T_KEY:notnull;
    public Task<RedisSortedSet<T>> GetSortedSet<T>(string key);
    public Task<T[]> GetSortedSetRangeByScore<T>(string key, int min, int max);
    public Task<RedisResult<T>> GetStringValue<T>(string key);

    public Task<bool> SetStringValue<T>(string key, T value);
    public Task<bool> SetHashValue<TKEY, T>(string key, TKEY subKey, T value)where T:class;
    
    
    
}