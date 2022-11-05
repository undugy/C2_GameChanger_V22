
using MySqlConnector;
using System.Security.Cryptography;
using System.Text;
using Server.Interface;
using StackExchange.Redis;

namespace Server.Services;

public class DBManager: IDBManager
{
    private static string _DBConnectionString;
    private static string _MasterDBConnectionString;
    public static void Init(IConfiguration configuration)
    {
        _DBConnectionString = configuration.GetSection("DBConnection")["v22"];
        _MasterDBConnectionString = configuration.GetSection("DBConnection")["v22_master"];
    }
    
    public async Task<MySqlConnection>GetDBConnection()
    {
        return await GetOpenMySqlConnection(_DBConnectionString);
    }

    public async Task<MySqlConnection> GetMasterDBConnection()
    {
        return await GetOpenMySqlConnection(_MasterDBConnectionString);
    }

    public async Task<MySqlConnection> GetOpenMySqlConnection(string connectionString)
    {
        var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();
        return connection;
    }
    
    
}