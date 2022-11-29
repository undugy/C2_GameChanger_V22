using Dapper;
using Server.Interface;
using Server.Services;

namespace Server.Model.User;

public class UserItem:IUserData
{
    public UInt32 ItemId{ get; set; }
    public UInt32 UserId{ get; set; }
    public UInt32 Quantity{ get; set; }
    public string? Kind{ get; set; }

    
    public (String,Object) InsertQuery()
    {
        var query = "INSERT INTO user_bag(ItemId,UserId,Quantity,Kind) " +
                       "VALUES(@itemId,@userId,@quantity,@kind)";
        var obj = new
        {
            itemId = ItemId,
            userId =UserId,
            quantity = Quantity,
            kind = Kind
        };
        
        return (query,obj);
    }

    public (String,Object) UpdateQuery()
    {
        var query = "INSERT INTO user_bag(UserId,ItemId,Quantity,Kind) " +
                    "VALUES (@itemId,@userId,@quantity,@kind)" +
                    "ON DUPLICATE KEY UPDATE Quantity=Quantity+@quantity";
        
        var obj=new
                {
                    itemId = ItemId,
                    userId =UserId,
                    quantity = Quantity,
                    kind = Kind
                };
        return (query,obj);
    }
    
    
    public static  (String,Object)  SelectUserItems(int userId)
    {
        var query="SELECT * FROM user_bag WHERE userId=@ID ";
        var obj=new { ID = userId };

        return (query,obj);
    }

}