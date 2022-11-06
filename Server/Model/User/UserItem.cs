using Dapper;
using Server.Interface;
using Server.Services;

namespace Server.Model.User;

public class UserItem
{
    public int ItemId{ get; set; }
    public int TeamId{ get; set; }
    public int Quantity{ get; set; }
    public string Kind{ get; set; }
    
    public (String,Object) InsertQuery()
    {
        var query = "INSERT INTO user_bag(ItemId,TeamId,Quantity,Kind) " +
                       "VALUES(@itemId,@teamId,@quantity,@kind)";
        var obj = new
        {
            itemId = ItemId,
            teamId =TeamId,
            quantity = Quantity,
            kind = Kind
        };
        
        return (query,obj);
    }

    public (String,Object) UpdateQuery()
    {
        
        var query = "UPDATE user_bag SET " +
                             "ItemId=@itemId," +
                             "TeamId=@teamId," +
                             "Quantity=@quantity,"+
                             "Kind=@kind "+
                             "WHERE TeamId=@teamId AND ItemId=@itemId";
        var obj=new
                {
                    itemId = ItemId,
                    teamId =TeamId,
                    quantity = Quantity,
                    kind = Kind
                };
                
          

        return (query,obj);
    }
    
    //TODO 다른곳 으로 옮겨서 static 필드 없애자 
   // public static async Task<Dictionary<int,BagProduct>> SelectQueryAsync(string userId)
   // {
   //     Dictionary<int,BagProduct> bagList;
   //     try
   //     {
   //         using (var conn = await DBManager.GetDBConnection())
   //         {
   //             var bagProducts =await conn.QueryAsync<BagProduct>("SELECT * FROM user_bag WHERE userId=@ID " ,new { ID = userId });
   //            
   //             
   //             bagList=bagProducts.Select((v, i) => (value: v, id: i))
   //                    .ToDictionary(pair => pair.value.ItemId, pair => pair.value);
   //         }
   //     }
   //     catch (Exception e)
   //     {
   //         Console.WriteLine(e);
   //         return null;
   //     }
   //     
   //     return bagList;
   // }

}