using CloudStructures;
using CloudStructures.Structures;

namespace Server.Interface;

public interface IRedisDatabase
{
    public Task<T> GetHashValue<Tkey,T>(string key, Tkey subKey)where Tkey:notnull;
   
    public RedisDictionary<T_KEY, T> GetHash<T_KEY, T>(string key)where T_KEY:notnull;
   
    public Task<T> GetStringValue<T>(string key);

    public Task<bool> SetStringValue<T>(string key, T value);
 
    
    
}