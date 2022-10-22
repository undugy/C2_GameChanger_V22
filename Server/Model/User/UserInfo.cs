using Dapper;
using Server.Interface;
using Server.Services;

namespace Server.Model.User;

public class UserInfo: IUserData
{
    public string id { get; set; }
    public string pw { get; set; }
    public string saltValue { get; set; }
    
    public Int32 ball { get; set; }
    public Int32 point { get; set; }
    public Int32 star { get; set; }
    public Int32 exp { get; set; }
    public Int32 level { get; set; }
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
                const string query = "UPDATE user_info SET " +
                                     "id=@ID," +
                                     "pw=@PW," +
                                     "saltValue=@SaltValue," +
                                     "ball=@Ball," +
                                     "point=@Point," +
                                     "star=@Star," +
                                     "exp=@Exp," +
                                     "level=@Level " +
                                     "WHERE id=@ID";
                result = await conn.ExecuteAsync(query,new
                {
                    ID=id,
                    PW=pw,
                    SaltValue=saltValue,
                    Ball=ball,
                    Point=point,
                    Star=star,
                    Exp=exp,
                    Level=level
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
        bool result=await RedisManager.SetHashValue<UserInfo>(id, nameof(UserInfo), new UserInfo()
        {
            id=id,
            pw=pw,
            saltValue = saltValue
        });
        return result;
    }

    public static async Task<UserInfo> SelectQueryOrDefaultAsync(string userId)
    {
        UserInfo? userInfo=null;
        try
        {
            using (var conn = await DBManager.GetDBConnection())
            {
               userInfo=await conn.QuerySingleOrDefaultAsync<UserInfo>("SELECT * FROM user_info WHERE id=@ID",
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
