using MySqlConnector;
using Server.Interface;

namespace Server.Services;

public class MasterDatabase:IDataBase
{
    private static string _connectionString;

    public static void Init(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public async Task<MySqlConnection>GetDBConnection()
    {
        
        return await GetOpenMySqlConnection(_connectionString);
    }
    
    public async Task<MySqlConnection> GetOpenMySqlConnection(string connectionString)
    {
        var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();
        return connection;
    }
}