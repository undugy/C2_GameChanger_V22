using Dapper;
using Server.Interface;
using Server.Services;

namespace Server.Model.User;

public class UserMail:IUserData
{
    public int MailId{ get; set; }
    public UInt32 UserId{ get; set; }
    public string ContentType{ get; set; }
    public UInt32 ItemId{ get; set; }
    public UInt32 Quantity{ get; set; }
    public DateTime ReceiveDate{ get; set; }
    
    public (String,Object) InsertQuery()
    {
        var query = "INSERT INTO user_mail(UserId,ContentType,ItemId,Quantity,ReceiveDate) " +
                                     "VALUES(@userId,@contentType,@itemId,@quantity,@receiveDate)";
        var obj =new
                {
                    userId=UserId,
                    contentType=ContentType,
                    itemId=ItemId,
                    quantity=Quantity,
                    receiveDate=ReceiveDate
                };
        return (query,obj);
    }



    //TODO 전체를 불러오는 쿼리는 여기 있으면 안됨
    // public Tuple<String, Object> SelectQueryAsync(string userId)
    // {
    //     var query = "SELECT * FROM user_mail WHERE TeamId=@ID ";
    //     var obj = new { ID = TeamId };
    //     return new Tuple<String, Object>(query, obj);
    // }
}