using Dapper;
using Server.Interface;
using Server.Services;

namespace Server.Model.User;

public class UserMail
{
    public int MailId{ get; set; }
    public int TeamId{ get; set; }
    public string ContentType{ get; set; }
    public int ItemId{ get; set; }
    public int Quantity{ get; set; }
    public DateTime ReceiveDate{ get; set; }
    
    public Tuple<String,Object> InsertQuery()
    {
        var query = "INSERT INTO user_mail(MailId,TeamId,ContentType,ItemId,Quantity,ReceiveDate) " +
                                     "VALUES(@userId,@teamId,@contentType,@itemId,@quantity,@receiveDate)";
        var obj =new
                {
                    mailId=MailId,
                    teamId=TeamId,
                    contentType=ContentType,
                    itemId=ItemId,
                    quantity=Quantity,
                    receiveDate=ReceiveDate
                };
        return new Tuple<string, object>(query,obj);
    }



    //TODO 전체를 불러오는 쿼리는 여기 있으면 안됨
    // public Tuple<String, Object> SelectQueryAsync(string userId)
    // {
    //     var query = "SELECT * FROM user_mail WHERE TeamId=@ID ";
    //     var obj = new { ID = TeamId };
    //     return new Tuple<String, Object>(query, obj);
    // }
}