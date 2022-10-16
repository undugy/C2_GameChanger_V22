using CloudStructures;
using System.Security.Cryptography;
using System.Text;
namespace Server.Services;
public class RedisManager
{
    public static RedisConnection s_redisConn { get; set; }
    private const string _allowableCharacters = "abcdefghijklmnopqrstuvwxyz0123456789";

    public static void Init(String address)
    {
        
            var config = new RedisConfig("basic", address);
            s_redisConn = new RedisConnection(config);
        
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