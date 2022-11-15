using CloudStructures;
using System.Security.Cryptography;
using System.Text;
using CloudStructures.Structures;
using CsvHelper.Configuration.Attributes;
using Microsoft.AspNetCore.Components.Web;
using Server.Interface;

namespace Server.Services;
public class RedisManager:IRedisManager
{
    private RedisConnection _redisConn;
    private const string _allowableCharacters = "abcdefghijklmnopqrstuvwxyz0123456789";

    //TODO 레디스의 Hash 자료구조를 이용해 유저정보 저장하기 

    public RedisConnection GetConnection() => _redisConn;
    private static RedisConfig _config;
    public static void Init(String address)
    {
        
        _config = new RedisConfig("basic", address);
         
        
    }

    public RedisManager()
    {
        _redisConn = new RedisConnection(_config);
    }
    
    public async Task<RedisResult<T>>GetHashValue<T>(string key,string subKey)
    {
        var redisId = new RedisDictionary<string,T>(GetConnection(),key,null);
        
        return await redisId.GetAsync(subKey);
    }
    public async Task<List<T>>GetListByRange<T>(string key)
    {
        var redisId = new RedisList<T>(GetConnection(),key,null);
        List<T> result = new List<T>();
        var redisList = await redisId.RangeAsync();
        result.AddRange(redisList.ToList());
        return result;
    }
    public  RedisDictionary<T_KEY,T>GetHash<T_KEY,T>(string key)
    {
        var redisId = new RedisDictionary<T_KEY,T>(GetConnection(),key,null);
        
        return redisId;
    }
    
    public async Task<RedisSortedSet<T>>GetSortedSet<T>(string key)
    {
        var redisId = new RedisSortedSet<T>(GetConnection(),key,null);
        await redisId.DeleteAsync();
        return redisId;
    }
    public async Task<T[]>GetSortedSetRangeByScore<T>(string key,int min,int max)
    {
        var redisId = new RedisSortedSet<T>(GetConnection(),key,null);
        await redisId.DeleteAsync();

        var result= await redisId.RangeByScoreAsync(min, max);
        
        return result;
    }
    public  async Task<RedisResult<T>> GetStringValue<T>(string key)
    {
        var redisId = new RedisString<T>(GetConnection(),key,null);
        
        return await redisId.GetAsync();
    }
    public  async Task<bool> SetStringValue<T>(string key, T value)
    {
        var defaultExpiry = TimeSpan.FromDays(1);
        var redisId = new RedisString<T>(GetConnection(),key,defaultExpiry);
        return await redisId.SetAsync(value);
    }
    public async Task<bool>SetHashValue<TKEY,T>(string key,TKEY subKey,T value)where T:class
    {
        var defaultExpiry = TimeSpan.FromDays(1);
        var redisId = new RedisDictionary<TKEY,T>(GetConnection(),key,defaultExpiry);
        var result= await redisId.SetAsync(subKey, value);
        return result;
    }
    public  async Task<long>InsertListValue<T>(string key,T value)where T:class
    {
        var defaultExpiry = TimeSpan.FromDays(1);
        var redisId = new RedisList<T>(GetConnection(),key,defaultExpiry);
        return await redisId.RightPushAsync(value);
    }
    
    public async Task<long>SetListValue<T>(string key,T value,TimeSpan?expiry=null)where T:class
    {
        var redisId = new RedisList<T>(GetConnection(),key,expiry);
        return await redisId.RightPushAsync(value);
    }
    
    
}

