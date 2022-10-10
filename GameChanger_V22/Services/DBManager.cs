
using MySqlConnector;

namespace GameChanger_V22.Services;

public class DBManager
{
    private static string _DBConnectionString;

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
    
    
}