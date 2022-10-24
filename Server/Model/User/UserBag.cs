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
    public UserBag(string id)
    {
        _id = id;
        _redisBagId = id + "bag";
        _redisMailId = id + "mail";
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

    public async Task<bool> MakeUserMail()
    {
        if (!await GetMailFromRedis())
        {
            var result= await UserMail.SelectQueryAsync(_id);
            if (result == null) return false;
            _userMails = result;
        }

        return true;
    }

    private async Task<bool> GetBagFromRedis()
    {
        var bagDict = await RedisManager.GetHash<int, BagProduct>(_redisBagId).GetAllAsync();
        if (bagDict == null)
            return false;
        _userBag = bagDict;
        return true;
    }
    
    private async Task<bool> GetMailFromRedis()
    {
        var mailList = await RedisManager.GetListByRange<UserMail>(_redisMailId);
        if (mailList == null)
            return false;
        _userMails = mailList;
        return true;
    }
    
        
    
}


