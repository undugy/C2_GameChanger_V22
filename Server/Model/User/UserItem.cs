using Dapper;
using Server.Interface;
using Server.Services;

namespace Server.Model.User;

public class UserItem
{
    public int ItemId{ get; set; }
    public UInt32 UserId{ get; set; }
    public int Quantity{ get; set; }
    public string Kind{ get; set; }

    
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
        
        var query = "UPDATE user_bag SET " +
                             "ItemId=@itemId," +
                             "UserId=@userId," +
                             "Quantity=@quantity,"+
                             "Kind=@kind "+
                             "WHERE TeamId=@teamId AND ItemId=@itemId";
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