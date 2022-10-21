using Dapper;
using Server.Interface;
using Server.Services;

namespace Server.Model.User;

public class UserInfo: IUserData
{
    public string id;
    public string pw;
    public string saltValue;
    public override string ToString()
    {
        return "User";
    }


    public async Task<ErrorCode> InsertUserInfo()
    {
        saltValue = DBManager.SaltString();
        var hashedPw = DBManager.MakeHashingPassWord(saltValue, pw);
        try
        {
            using (var conn=await DBManager.GetDBConnection())
            {
                var row = await conn.ExecuteAsync(
                    @"INSERT INTO user_info(id,pw,saltValue) Values(@Id,@Pw,@salt) ",
                    new
                    {
                        Id=id,
                        Pw=hashedPw,
                        salt=saltValue
                    });
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ErrorCode.CREATE_FAIL;
        }

        return ErrorCode.NONE;
    }
    
    public async Task<bool> SaveDataToDB()
    {
        int result = 0;
        try
        {
            using (var conn = await DBManager.GetDBConnection())
            {
                result = await conn.ExecuteAsync(
                    @"update user_info set id=@id,pw=@pw,saltValue=@saltValue)");
                
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
        bool result=await RedisManager.SetHashValue<UserInfo>(id, nameof(UserInfo), new UserInfo()
        {
            id=id,
            pw=pw,
            saltValue = saltValue
        });
        return result;
    }

    
}