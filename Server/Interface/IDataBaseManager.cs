using MySqlConnector;

namespace Server.Interface;

public interface IDataBaseManager
{
    public Task<MySqlConnection> GetOpenMySqlConnection(string connectionString);
    public Task<MySqlConnection> GetAccountDbConnection();
}