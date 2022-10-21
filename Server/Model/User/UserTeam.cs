using Dapper;
using Server.Interface;
using Server.Services;

namespace Server.Model.User;

public class UserTeam:IUserData
{
    public Int32 id;
    public string  userId;
    public string? nickName;
    public string  intro;
    public Int32   leagueId;

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
    public async Task<bool> InsertUserTeam()
    {
        int result=0;
        try
        {
            using (var conn = await DBManager.GetDBConnection())
            {
                const string query = "INSERT INTO user_team(id,userId,nickName) " +
                                     "VALUES(@ID,@UserId,@NickName)";
                result = await conn.ExecuteAsync(query,new
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
            //throw;
        }

        return (result == 1);
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
        bool result=await RedisManager.SetHashValue<UserTeam>(userId, nameof(UserTeam), new UserTeam()
        {
            id=id,
            userId=userId,
            nickName=nickName,
            intro=intro,
            leagueId=leagueId
        });
        return result;
    }
}