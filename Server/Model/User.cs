using Dapper;
using Microsoft.AspNetCore.SignalR;
using Server.Interface;
using Server.Services;

namespace Server.Model;

public class User: IUserData
{
    public string id;
    public string pw;
    public string saltValue;
    public override string ToString()
    {
        return "User";
    }

    public async Task<bool> SaveDataToDB()
    {
        int result = 0;
        try
        {
            using (var conn = await DBManager.GetDBConnection())
            {
                result = await conn.ExecuteAsync(
                    @"update user set id=@id,pw=@pw,saltValue=@saltValue)");
                
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
        bool result=await RedisManager.SetHashValue<User>(id, nameof(User), new User()
        {
            id=id,
            pw=pw,
            saltValue = saltValue
        });
        return result;
    }

    
}