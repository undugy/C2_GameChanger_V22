using Dapper;
using Server.Interface;
using Server.Services;

namespace Server.Model.User;

public class UserBag:IUserData
{
    public int itemId;
    public string userId;
    public int quantity;
    public string kind;

   
    
    public async Task<ErrorCode> InsertUserMail()
    {
        var result=ErrorCode.NONE;
        int row = 0;
        try
        {
            using (var conn = await DBManager.GetDBConnection())
            {
                const string query = "INSERT INTO user_bag(itemId,userId,quantity,kind) " +
                                     "VALUES(@ItemId,@UserId,@Quantity,@Kind)";
                row = await conn.ExecuteAsync(query,new
                {
                    ItemId=itemId,
                    UserId=userId,
                    Quantity=quantity,
                    Kind=kind
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
        int result = 0;
        try
        {
            using (var conn = await DBManager.GetDBConnection())
            {
                const string query = "UPDATE user_bag SET " +
                                     "itemId=@ItemID," +
                                     "userId=@UserID," +
                                     "quantity=@Quantity,"+
                                     "kind=@Kind "+
                                     "WHERE userId=@UserId";
                result = await conn.ExecuteAsync(query,new
                {
                    ItemID=itemId,
                    UserID=userId,
                    Quantity=quantity,
                    Kind=kind
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
        //TODO hash id 만들어서 넣어주기
        return true;
    }
    
    public static async Task<UserBag> SelectQueryFirstAsync(string userId,int itemId)
    {
        UserBag userMail=null;
        try
        {
            using (var conn = await DBManager.GetDBConnection())
            {
                userMail=await conn.QueryFirstAsync<UserBag>("SELECT * FROM user_bag WHERE userId=@ID " +
                                                             "AND itemId=@ItemId",new { ID = userId,ItemId=@itemId });
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return userMail;
        }

        return userMail;
    }
    
    public static async Task<List<UserBag>> SelectQueryAsync(string userId)
    {
        List<UserBag> mailList = new List<UserBag>();
        try
        {
            using (var conn = await DBManager.GetDBConnection())
            {
                var mail =await conn.QueryAsync<UserBag>("SELECT * FROM user_bag WHERE userId=@ID " ,new { ID = userId });
                mailList.AddRange(mail.ToList());
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }

        return mailList;
    }
}