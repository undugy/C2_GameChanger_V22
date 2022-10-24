using CloudStructures.Structures;
using Dapper;
using Server.Interface;
using Server.Services;

namespace Server.Model.User;

public class UserBag
{
   
    private readonly string _id;
    private readonly string _redis_id;
    private Dictionary<int,BagProduct> _userBag;

    public UserBag(string id)
    {
        _id = id;
        _redis_id = id + "bag";
    }

    public async Task<bool>MakeUserBag()
    {
        if (!await GetBagFromRedis())
        {
            var result= await BagProduct.SelectQueryAsync(_id);
            if (result == null) return false;
            _userBag = result;
        }

        return true;
    }
    
    private async Task<bool> GetBagFromRedis()
    {
        var bagDict = await RedisManager.GetHash<int, BagProduct>(_redis_id).GetAllAsync();
        if (bagDict == null)
            return false;
        _userBag = bagDict;
        return true;
    }
    

    
        
    
}


