using Dapper;
using MySqlConnector;
using Server.Interface;
using Server.Model.User;

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
    
    public async Task<Tuple<ErrorCode,int>> SelectSingleItemId(string itemName)
    {
        ErrorCode errorCode = ErrorCode.NONE;
        int itemId = -1;
        await using (var connection= await GetDBConnection())
        {
            
            try
            {
                itemId = await
                    connection.QuerySingleOrDefaultAsync<int>("SELECT ItemId FROM item WHERE Name=@name",
                        new { name = itemName });
            }
            catch (Exception e)
            {
                errorCode = ErrorCode.NOID;
            }
            
        }

        return new Tuple<ErrorCode,int>(errorCode,itemId);
    }
    
    public async Task<Tuple<ErrorCode,uint>> SelectSingleTeamId(string teamName)
    {
        ErrorCode errorCode = ErrorCode.NONE;
        uint teamId = 0;
        await using (var connection= await GetDBConnection())
        {
            try
            {
                teamId = await
                    connection.QuerySingleOrDefaultAsync<uint>("SELECT TeamId FROM team WHERE Name=@name",
                        new { name = teamName });
            }
            catch (Exception e)
            {
                errorCode = ErrorCode.NOID;
            }
            
        }

        return new Tuple<ErrorCode,uint>(errorCode,teamId);
    }
}