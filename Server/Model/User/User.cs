using Server.Interface;
using Server.Services;

namespace Server.Model.User;

public class User
{
    private string _id;
    private Dictionary<string,IUserData> _userDatas;

    public User(string userId)
    {
        _id = userId;
        _userDatas = new Dictionary<string, IUserData>();
    }
    
    public async Task<bool> CreateUser(string pw)
    {
        //TODO 여기서 레디스랑 DB조사
        var userInfo = GetRedisData<UserInfo>();//new UserInfo(){id=id,pw=pw};
        if (userInfo != null)
        {
            return false;
        }
        var DBuserInfo = await UserInfo.SelectQueryOrDefaultAsync(_id);
        if (DBuserInfo != null)
        {
            return false;
        }
        
        UserInfo newUserInfo = new UserInfo(){id=_id,pw=pw};
        var result = await  newUserInfo.InsertUserInfo();
        if (ErrorCode.NONE == result)
        {
            return AddUserData<UserInfo>(newUserInfo);
        }
        
        return false;
    }

    private async Task<T> GetRedisData<T>()
    {
        var result = await RedisManager.GetHashValue<T>(_id, nameof(T));
        return result.Value;
    }

    private bool AddUserData<T>(IUserData value) where T : class
    {
        return _userDatas.TryAdd(nameof(T), value);
    }

    public async Task<bool>UpdateUserDatas()
    {
        foreach (var data in _userDatas)
        {
            if (!await data.Value.SaveDataToRedis())
            {
                //예외처리 보류
                var result= await data.Value.SaveDataToDB();
            }
        }

        return true;
    }
}