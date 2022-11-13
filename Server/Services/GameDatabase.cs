using System.ComponentModel;
using Dapper;
using MySqlConnector;
using Server.Interface;
using Server.Model.User;

namespace Server.Services;

public class GameDatabase:IDataBase
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

    public async Task<Tuple<ErrorCode,UserInfo>> SelectSingleUserInfo(string email)
    {
        
        UserInfo? userInfo=null;
        ErrorCode errorCode = ErrorCode.NONE;
        await using (var connection= await GetDBConnection())
        {
           
            try
            {
                userInfo = await connection.QuerySingleOrDefaultAsync<UserInfo>(
                    "SELECT * FROM user_info WHERE Email=@Usermail",
                    new { Usermail = email });
            }
            catch (Exception e)
            {
                errorCode = ErrorCode.NOID;
            }
            
        }

        return new Tuple<ErrorCode,UserInfo>(errorCode,userInfo);
    }
}

