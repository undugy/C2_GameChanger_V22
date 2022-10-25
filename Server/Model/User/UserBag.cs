using CloudStructures.Structures;
using Dapper;
using Server.Interface;
using Server.Services;

namespace Server.Model.User;

public class UserBag
{
   
    private readonly string _id;
    private readonly string _redisBagId;
    private readonly string _redisMailId;
    private Dictionary<int,BagProduct> _userBag;
    private List<UserMail> _userMails;

    public Dictionary<int,BagProduct>GetUserBag() => _userBag; 
    public List<UserMail>GetUserMail()=> _userMails;
    public UserBag(string id)
    {
        _id = id;
        _redisBagId = id + "bag";
        _redisMailId = id + "mail";
    }

    public async Task<bool> SetUpBagAndMail()
    {
        if (!await LoadUserBag())
        {
            return false;
        }

        if (!await LoadUserMail())
        {
            return false;
        }

        return true;
    }
    private async Task<bool>LoadUserBag()
    {
       if (!await GetBagFromRedis())
       {
            var result= await BagProduct.SelectQueryAsync(_id);
            if (result == null) return false;
            _userBag = result;
            foreach (var products in result)
            {
                await products.Value.SaveDataToRedis(_redisBagId);

            }
        }

        return true;
    }

    
    
    private async Task<bool> LoadUserMail()
    {
        if (!await GetMailFromRedis())
        {
            var result= await UserMail.SelectQueryAsync(_id);
            if (result == null) return false;
            _userMails = result;
            foreach (var products in result)
            {
                
            }
        }

        return true;
    }

    private async Task<bool> GetBagFromRedis()
    {
        
        
        var bagDict = await RedisManager.GetHash<int, BagProduct>(_redisBagId).GetAllAsync();
        if (bagDict.Count == 0)
            return false;
        _userBag = bagDict;
        return true;
    }
    
    private async Task<bool> GetMailFromRedis()
    {
        var mailList = await RedisManager.GetSortedSetRangeByScore<UserMail>(_redisMailId,0,-1);
        if (mailList == null)
            return false;
        _userMails = mailList.ToList();
        return true;
    }
    
        
    
}


