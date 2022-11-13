using MySqlConnector;

namespace Server.Interface;

public interface IDataBase
{
    public Task<MySqlConnection> GetOpenMySqlConnection(string connectionString);
    public Task<MySqlConnection> GetDBConnection();
}