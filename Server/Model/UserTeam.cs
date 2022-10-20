using CloudStructures;
using Server.Interface;
using Server.Services;
using Dapper;
namespace Server.Model;

public class UserTeam:IUserData
{
    public Int32 id;
    public string  userId;
    public string? nickName;
    public string  intro;
    public string  teamName;
    public Int32   teamLevel;
    public Int32   exp;
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
                const string query = "SELECT COUNT(*) FROM user_team WHERE teamName=@TeamName";
                result = await conn.ExecuteScalarAsync<Int64>(query,new
                {
                    TeamName=teamName
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
                const string query = "INSERT INTO user_team(id,userId,nickName,teamName) " +
                                     "VALUES(@ID,@UserId,@NickName,@TeamName)";
                result = await conn.ExecuteAsync(query,new
                {
                    ID=id,
                    UserId=userId,
                    NickName=nickName,
                    TeamName=teamName
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
                                     "SET id=@id," +
                                     "userId=@userId," +
                                     "nickName=@nickName," +
                                     "intro=@intro," +
                                     "teamName=@teamName," +
                                     "teamLevel=@teamLevel," +
                                     "exp=@exp," +
                                     "leagueId=@leagueId " +
                                     "WHERE userId=@userId";
                result = await conn.ExecuteAsync(query);
                
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
            teamName=teamName,
            teamLevel=teamLevel,
            exp=exp,
            leagueId=leagueId
        });
        return result;
    }
}