using CloudStructures;
using System.Security.Cryptography;
using System.Text;
using CsvHelper.Configuration.Attributes;

namespace Server.Services;
public class RedisManager
{
    private static RedisConnection _redisConn { get; set; }
    private const string _allowableCharacters = "abcdefghijklmnopqrstuvwxyz0123456789";

    //TODO 레디스의 Hash 자료구조를 이용해 유저정보 저장하기 

    public static RedisConnection GetConnection() => _redisConn;
    public static void Init(String address)
    {
        
         var config = new RedisConfig("basic", address);
         var converter = new CloudStructures.Converters.MessagePackConverter();
         _redisConn = new RedisConnection(config,converter);
        
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

