
using MySqlConnector;
using System.Security.Cryptography;
using System.Text;
namespace GameChanger_V22.Services;

public class DBManager
{
    private static string _DBConnectionString;
    const string _allowableCharacters = "abcdefghijklmnopqrstuvwxyz0123456789";
    public static void Init(string adress)
    {
        _DBConnectionString = adress;
    }
    
    public static async Task<MySqlConnection>GetDBConnection()
    {
        return await GetOpenMySqlConnection(_DBConnectionString);
    }

    private static async Task<MySqlConnection> GetOpenMySqlConnection(string connectionString)
    {
        var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();
        return connection;
    }
    public static string MakeHashingPassWord(string saltvalue, string pw)
    {
        var sha = new SHA256Managed();
        byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(saltvalue + pw));
        StringBuilder stringBuilder = new StringBuilder();
        foreach (byte b in hash)
        {
            stringBuilder.AppendFormat("{0:x2}", b);//헥사값(16진수)으로 변환에서 넣어준다.-> 유니코드는 16비트로 문자표현
        }

        return stringBuilder.ToString();
    }
    public static string SaltString() //암호화를 더 견고하게 해주는 값
    {
        var bytes = new byte[64];
        using (var random = RandomNumberGenerator.Create())
        {
            random.GetBytes(bytes);
        }
        return new string(bytes.Select(x => _allowableCharacters[x % _allowableCharacters.Length]).ToArray());
    }
    
}