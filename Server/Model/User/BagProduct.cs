using Dapper;
using Server.Interface;
using Server.Services;

namespace Server.Model.User;

public class BagProduct
{
    public int itemId{ get; set; }
    public string userId{ get; set; }
    public int quantity{ get; set; }
    public string kind{ get; set; }
    
    public async Task<ErrorCode> InsertBagProduct()
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
                                     "WHERE userId=@UserID AND itemId=@ItemID";
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
    
    public async Task<bool> SaveDataToRedis(string redis_id)
    {
        return await RedisManager.SetHashValue(redis_id, itemId, this);
    }
    
    public static async Task<BagProduct> SelectQueryFirstAsync(string userId,int itemId)
    {
        BagProduct bagProduct=null;
        try
        {
            using (var conn = await DBManager.GetDBConnection())
            {
                bagProduct=await conn.QueryFirstAsync<BagProduct>("SELECT * FROM user_bag WHERE userId=@ID " +
                                                                  "AND itemId=@ItemId",new { ID = userId,ItemId=@itemId });
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return bagProduct;
        }

        return bagProduct;
    }
    
    public static async Task<Dictionary<int,BagProduct>> SelectQueryAsync(string userId)
    {
        Dictionary<int,BagProduct> bagList;
        try
        {
            using (var conn = await DBManager.GetDBConnection())
            {
                var bagProducts =await conn.QueryAsync<BagProduct>("SELECT * FROM user_bag WHERE userId=@ID " ,new { ID = userId });
               
                
                bagList=bagProducts.Select((v, i) => (value: v, id: i))
                       .ToDictionary(pair => pair.value.itemId, pair => pair.value);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
        
        return bagList;
    }

}