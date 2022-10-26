using Dapper;
using Server.Interface;
using Server.Services;

namespace Server.Model.User;

public class UserMail
{
    public string userId{ get; set; }
    public int checkDay{ get; set; }
    public string contentType{ get; set; }
    public int itemId{ get; set; }
    public int quantity{ get; set; }
    public DateTime receiveDate{ get; set; }
    
    public async Task<ErrorCode> InsertUserMail()
    {
        var result=ErrorCode.NONE;
        int row = 0;
        try
        {
            using (var conn = await DBManager.GetDBConnection())
            {
                const string query = "INSERT INTO user_mail(userId,checkDay,contentType,itemId,quantity,receiveDate) " +
                                     "VALUES(@UserId,@CheckDay,@ContentType,@ItemId,@Quantity,@ReceiveDate)";
                row = await conn.ExecuteAsync(query,new
                {
                    UserId=userId,
                    CheckDay=checkDay,
                    ContentType=contentType,
                    ItemId=itemId,
                    Quantity=quantity,
                    ReceiveDate=receiveDate
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
        return true;
    }

    public async Task<bool> SaveDataToRedis()
    {
        //TODO hash id 만들어서 넣어주기
        return true;
    }
    
    public static async Task<UserMail> SelectQueryFirstAsync(string userId,int day,string contentType)
    {
        UserMail userMail=null;
        try
        {
            using (var conn = await DBManager.GetDBConnection())
            {
                userMail=await conn.QueryFirstAsync<UserMail>("SELECT * FROM user_mail WHERE userId=@ID " +
                                                              "AND checkDay=@CheckDay "+
                                                              "AND contentType=@ContentType",new { ID = userId,CheckDay=day,ContentType=contentType });
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return userMail;
        }

        return userMail;
    }
    
    public static async Task<List<UserMail>> SelectQueryAsync(string userId)
    {
        List<UserMail> mailList = new List<UserMail>();
        try
        {
            using (var conn = await DBManager.GetDBConnection())
            {
                var mail =await conn.QueryAsync<UserMail>("SELECT * FROM user_mail WHERE userId=@ID " ,new { ID = userId });
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