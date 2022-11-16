using System.ComponentModel;
using Dapper;
using MySqlConnector;
using Server.Interface;
using Server.Model.ReqRes;
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

    public async Task<SetUpResponse> MakeSetUpResponse(UInt32 userId)
    {
        var result = new SetUpResponse();
        string query = "SELECT * FROM user_team WHERE UserId=@id;" +
                       " SELECT * FROM user_mail WHERE UserId=@id";
        await using (var connection = await GetDBConnection())
        {
            try
            {
                using(var multi= await connection.QueryMultipleAsync(query,new{id=userId}))
                {
                    result.TeamInfo = multi.Read<UserTeam>().Single();
                    result.MailList= multi.Read<UserMail>().ToList();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result.Result = ErrorCode.CREATE_FAIL;
            }
            
        }

        return result;

    }
}

