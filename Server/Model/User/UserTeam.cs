using Dapper;
using Server.Interface;
using Server.Services;

namespace Server.Model.User;

public class UserTeam:IUserData
{
    public Int32 id { get; set; }
    public string  userId{ get; set; }
    public string? nickName{ get; set; }
    public string  intro{ get; set; }
    public Int32   leagueId{ get; set; }

    public override string ToString()
    {
        return "UserTeam";
    }

    public async Task<Int64> CountTeamFromDB(string teamName)
    {
        Int64 result = 0;
        try
        {
            using (var conn = await DBManager.GetDBConnection())
            {
                const string query = "SELECT COUNT(*) FROM user_team WHERE id=@ID";
                result = await conn.ExecuteScalarAsync<Int64>(query,new
                {
                    ID=id
                });
                
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //throw;
        }

        return result;
    }
    public async Task<ErrorCode> InsertUserTeam()
    {
        var result=ErrorCode.NONE;
        int row = 0;
        try
        {
            using (var conn = await DBManager.GetDBConnection())
            {
                const string query = "INSERT INTO user_team(id,userId,nickName) " +
                                     "VALUES(@ID,@UserId,@NickName)";
                row = await conn.ExecuteAsync(query,new
                {
                    ID=id,
                    UserId=userId,
                    NickName=nickName
                });
                
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            result = ErrorCode.CREATE_FAIL;
        }

        return result;
    }
    public async Task<bool> SaveDataToDB()
    {
        int result=0;
        try
        {
            using (var conn = await DBManager.GetDBConnection())
            {
                const string query = "UPDATE user_team " +
                                     "SET id=@Id," +
                                     "userId=@UserId," +
                                     "nickName=@NickName," +
                                     "intro=@Intro," +
                                     "leagueId=@LeagueId " +
                                     "WHERE userId=@UserId";
                result = await conn.ExecuteAsync(query,new
                {
                    Id=id,
                    UserId=userId,
                    NickName=nickName,
                    Intro=intro,
                    LeagueId=leagueId
                });
                
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //throw;
        }

        return (result == 1);
    }
    
    public async Task<bool> SaveDataToRedis()
    {
        bool result=await RedisManager.SetHashValue<string,UserTeam>(userId, nameof(UserTeam), new UserTeam()
        {
            id=id,
            userId=userId,
            nickName=nickName,
            intro=intro,
            leagueId=leagueId
        });
        return result;
    }
    
    public static async Task<UserTeam> SelectQueryOrDefaultAsync(string userId)
    {
        UserTeam? userInfo=null;
        try
        {
            using (var conn = await DBManager.GetDBConnection())
            {
                userInfo=await conn.QuerySingleOrDefaultAsync<UserTeam>("SELECT * FROM user_team WHERE id=@ID",
                    new { ID = userId });
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return userInfo;
        }

        return userInfo;
    }
}