using System.ComponentModel;
using Dapper;
using MySqlConnector;
using Server.Interface;
using Server.Model.ReqRes;
using Server.Model.User;
using Server.Table;

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

    public async Task<Tuple<ErrorCode,UserInfo?>> SelectSingleUserInfo(string email)
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

        return new Tuple<ErrorCode,UserInfo?>(errorCode,userInfo);
    }
    public async Task<Tuple<ErrorCode,DateTime>> SelectUserLastAccess(UInt32 userId)
    {
        
        DateTime lastAccess=DateTime.UnixEpoch;
        ErrorCode errorCode = ErrorCode.NONE;
        await using (var connection= await GetDBConnection())
        {
           
            try
            {
                lastAccess = await connection.QuerySingleOrDefaultAsync<DateTime>(
                    "SELECT LastAccess FROM user_log WHERE UserId=@id",
                    new { id = userId });
            }
            catch (Exception e)
            {
                errorCode = ErrorCode.NOID;
            }
            
        }

        return new Tuple<ErrorCode,DateTime>(errorCode,lastAccess);
    }
    public async Task<Tuple<ErrorCode,UserAttendance>> SelectSingleUserAttendance(UInt32 userId,string contentType)
    {
        
        UserAttendance? userAttendance=null;
        ErrorCode errorCode = ErrorCode.NONE;
        await using (var connection= await GetDBConnection())
        {
           
            try
            {
                userAttendance = await connection.QuerySingleOrDefaultAsync<UserAttendance>(
                    "SELECT * FROM user_attendance WHERE UserId=@userid AND ContentType=@contenttype",
                    new { userid = userId,contenttype=contentType });
                if (userAttendance == null) errorCode = ErrorCode.NOT_INIT;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                errorCode = ErrorCode.NOID;
            }
            
        }

        return new Tuple<ErrorCode,UserAttendance>(errorCode,userAttendance);
    }
    
    public async Task<ErrorCode> UpdateUserLastAccess(UInt32 userId,DateTime lastAccess)
    {
        
        
        ErrorCode errorCode = ErrorCode.NONE;
        await using (var connection= await GetDBConnection())
        {
           
            try
            {
                var result= await connection.ExecuteAsync(
                    "UPDATE user_log SET LastAccess=@access WHERE UserId=@id",
                    new { id = userId, access=lastAccess });
            }
            catch (Exception e)
            {
                errorCode = ErrorCode.NOID;
            }
            
        }

        return errorCode;
    }
    public async Task<SetUpResponse> MakeSetUpResponse(UInt32 userId)
    {
        var result = new SetUpResponse();
        string query = "SELECT * FROM user_team WHERE UserId=@id;" +
                       " SELECT * FROM user_mail WHERE UserId=@id;" +
                       " SELECT * FROM user_attendance WHERE UserId=@id AND ContentType=@type";
        await using (var connection = await GetDBConnection())
        {
            try
            {
                var multi = await connection.QueryMultipleAsync(query, new { id = userId, type = "dailyCheckIn" });
                
                    result.TeamInfo = multi.Read<UserTeam>().Single();
                    result.MailList= multi.Read<UserMail>().ToList();
                    result.CheckInList = multi.Read<UserAttendance>().Single();

                multi.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result.Result = ErrorCode.CREATE_FAIL;
            }
            
        }

        return result;

    }

    public async Task<PkCheckInResponse> MakeCheckInResponse(UInt32 id,UInt32 itemId,uint quantity,string itemName)
    {
        await using (var connection = await GetDBConnection())
        {
            if (itemId < 100)
            {
                var query = "UPDATE user_team SET " + itemName + "=" + itemName + "+@num"+
                            " WHERE UserId=@userId";
                var result = await connection.ExecuteAsync(query, new { userId = id ,num=quantity});
                if (result == 0)
                {
                    throw new Exception("재화 받기 실패");
                }
            }
            else
            {
                var userItem = new UserItem() { ItemId = itemId, Kind = "item", Quantity = quantity, UserId = id };
                var query = userItem.UpdateQuery();
                var result = await connection.ExecuteAsync(query.Item1, query.Item2);
                if (result == 0)
                {
                    throw new Exception("아이템 받기 실패");
                }
            }
        }

        return new PkCheckInResponse() { RewardName = itemName, RewardQuantity = quantity };



    }
}

