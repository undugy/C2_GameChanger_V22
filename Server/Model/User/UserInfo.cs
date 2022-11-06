using Dapper;
using Server.Interface;
using Server.Services;
using Server.Table;

namespace Server.Model.User;

public class UserInfo: IUserData
{
    public string UserId { get; set; }
    public string Email { get; set; }
    public string SaltValue { get; set; }
    public string HashedPassword { get; set; }
    
    public override string ToString()
    {
        return "user_info";
    }

    
    

    public (String,Object) InsertQuery()
    {
        var query = "INSERT INTO user_info(Email,HashedPassword,SaltValue) Values(@email,@pw,@salt) ";
        var obj= new
                    {
                        email=Email,
                        pw=HashedPassword,
                        salt=SaltValue
                    };
        
        return (query,obj);
    }


    public (String,Object) UpdateQuery()
    {
       
                var query = "UPDATE user_info SET " +
                                     "Email=@email," +
                                     "HashedPassword=@PW," +
                                     "SaltValue=@saltValue "+
                                     "WHERE UserId=@userId";
                var obj= new
                {
                    userId=UserId,
                    email=Email,
                    PW=HashedPassword,
                    saltValue=SaltValue
                };
          

        return (query,obj);
    }
    

    // public static async Task<UserInfo> SelectQueryOrDefaultAsync(string userId)
    // {
    //     UserInfo? userInfo=null;
    //     try
    //     {
    //         using (var conn = await DBManager.GetDBConnection())
    //         {
    //            userInfo=await conn.QuerySingleOrDefaultAsync<UserInfo>("SELECT * FROM user_info WHERE id=@ID",
    //                 new { ID = userId });
    //         }
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine(e);
    //         return userInfo;
    //     }
    //
    //     return userInfo;
    // }

    
}
