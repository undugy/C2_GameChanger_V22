
using MySqlConnector;
using System.Security.Cryptography;
using System.Text;
using Server.Interface;
using StackExchange.Redis;

namespace Server.Services;

public class DBManager: IDataBaseManager
{
    private static string _DBConnectionString;
   
    public static void Init(IConfiguration configuration)
    {
        _DBConnectionString = configuration.GetSection("DBConnection")["MySqlGame"];
    }
    
    public async Task<MySqlConnection>GetDBConnection()
    {
        return await GetOpenMySqlConnection(_DBConnectionString);
    }

    public async Task<MySqlConnection> GetOpenMySqlConnection(string connectionString)
    {
        var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();
        return connection;
    }
    
    
}