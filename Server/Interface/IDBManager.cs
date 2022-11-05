using MySqlConnector;

namespace Server.Interface;

public interface IDBManager
{
    public Task<MySqlConnection> GetOpenMySqlConnection(string connectionString);
    public Task<MySqlConnection> GetDBConnection();
    public Task<MySqlConnection> GetMasterDBConnection();
}