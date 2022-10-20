using CloudStructures;
using System.Security.Cryptography;
using System.Text;
using CloudStructures.Structures;
using CsvHelper.Configuration.Attributes;
using Microsoft.AspNetCore.Components.Web;

namespace Server.Services;
public class RedisManager
{
    private static RedisConnection _redisConn;
    private const string _allowableCharacters = "abcdefghijklmnopqrstuvwxyz0123456789";

    //TODO 레디스의 Hash 자료구조를 이용해 유저정보 저장하기 

    public static RedisConnection GetConnection() => _redisConn;
    public static void Init(String address)
    {
        
         var config = new RedisConfig("basic", address);
         var converter = new CloudStructures.Converters.MessagePackConverter();
         _redisConn = new RedisConnection(config,converter);
        
    }
    public static async Task<RedisResult<T>>GetHashValue<T>(string key,string subKey,T value)where T:class
    {
        var redisId = new RedisDictionary<string,T>(RedisManager.GetConnection(),key,null);
        return await redisId.GetAsync(subKey);
    }
    public static async Task<RedisResult<T>> GetStringValue<T>(string key)
    {
        var redisId = new RedisString<T>(RedisManager.GetConnection(),key,null);
        
        return await redisId.GetAsync();
    }
    public static async Task<bool> SetStringValue<T>(string key, T value)
    {
        var defaultExpiry = TimeSpan.FromDays(1);
        var redisId = new RedisString<T>(RedisManager.GetConnection(),key,defaultExpiry);
        return await redisId.SetAsync(value);
    }
    public static async Task<bool>SetHashValue<T>(string key,string subKey,T value)where T:class
    {
        var defaultExpiry = TimeSpan.FromDays(1);
        var redisId = new RedisDictionary<string,T>(RedisManager.GetConnection(),key,defaultExpiry);
        return await redisId.SetAsync(subKey, value);
    }
    
    
    
    public static string AuthToken()
    {
        var bytes = new byte[25];
        using (var random = RandomNumberGenerator.Create())
        {
            random.GetBytes(bytes);
        }
        return new string(bytes.Select(x => _allowableCharacters[x % _allowableCharacters.Length]).ToArray());
    }
}

