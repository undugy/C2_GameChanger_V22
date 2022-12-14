using System.Security.Cryptography;
using System.Text;

namespace Server;

public static class HashFunctions
{
    private static string  _allowableCharacters = "abcdefghijklmnopqrstuvwxyz0123456789";
    public static string  MakeHashingPassWord(string saltValue, string pw)//임시
    {
        var sha = SHA256.Create();
        byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(saltValue + pw));
        StringBuilder stringBuilder = new StringBuilder();
        foreach (byte b in hash)
        {
            stringBuilder.AppendFormat("{0:x2}", b);//헥사값(16진수)으로 변환에서 넣어준다.-> 유니코드는 16비트로 문자표현
        }

        return stringBuilder.ToString();
    }
    public static string  SaltString() //암호화를 더 견고하게 해주는 값
    {
        var bytes = new byte[64];
        using (var random = RandomNumberGenerator.Create())
        {
            random.GetBytes(bytes);
        }
        return new string(bytes.Select(x => _allowableCharacters[x % _allowableCharacters.Length]).ToArray());
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