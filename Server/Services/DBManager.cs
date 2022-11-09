
using MySqlConnector;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using Server.Interface;
using Server.Model.User;
using StackExchange.Redis;
using ZLogger;

namespace Server.Services;

public class DBManager: IDBManager
{
    private static string _DBConnectionString;
    private static string _masterDBConnectionString;
    private readonly ILogger _logger;
    public static void Init(IConfiguration configuration)
    {
        _DBConnectionString = configuration.GetSection("DBConnection")["v22"];
        _masterDBConnectionString = configuration.GetSection("DBConnection")["v22_master"];
    }

    public DBManager(ILogger<DBManager> logger)
    {
        _logger = logger;
    }
    
    public async Task<MySqlConnection>GetDBConnection()
    {
        return await GetOpenMySqlConnection(_DBConnectionString);
    }

    public async Task<MySqlConnection> GetMasterDBConnection()
    {
        return await GetOpenMySqlConnection(_masterDBConnectionString);
    }

    public async Task<MySqlConnection> GetOpenMySqlConnection(string connectionString)
    {
        var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();
        return connection;
    }

    public void sex()
    {
        
    }

}