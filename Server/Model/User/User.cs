using CloudStructures.Structures;
using Server.Interface;
using Server.Services;
using Server.Table.CsvImpl;

namespace Server.Model.User;

public class User
{
    private readonly string _id;
    private Dictionary<string,IUserData> _userDatas;
    //private Dictionary<int,UserBag> _bagList;
   
    public User(string userId)
    {
        _id = userId;
        _userDatas = new Dictionary<string, IUserData>();
        
    }

    
    public async Task<ErrorCode> CreateUser(string pw)
    {
        //TODO 여기서 레디스랑 DB조사
        var userInfo = await BringTable<UserInfo>();//
        
        var result = ErrorCode.NONE;
        if (userInfo != null)
        {
            result = ErrorCode.ALREADY_EXIST;
            return result;
        }
        userInfo = await UserInfo.SelectQueryOrDefaultAsync(_id);
        if (userInfo != null)
        {
            result = ErrorCode.ALREADY_EXIST;
            return result;
        }
        
        userInfo = new UserInfo(){id=_id,pw=pw};;
        userInfo.id = _id;
        userInfo.pw = pw;
        result = await  userInfo.InsertUserInfo();
        await userInfo.SaveDataToRedis();
     
        
        return result;
    }

    public async Task<bool> SetUpUserData()
    {

        if (!await GetUserInfo())
        {
            return false;
        }
        if (!await GetUserTeam())
        {
            return false;
        }

        return true;
    }

    private async Task<bool> GetUserInfo()
    {
        var userInfo = await BringTable<UserInfo>();
        if(userInfo==null)
        {
            userInfo=await UserInfo.SelectQueryOrDefaultAsync(_id);
            if (userInfo == null)
                return false;
            AddUserData<UserInfo>(userInfo);
        }

        return true;
    }
    //private async Task<bool> GetUserBag()
    //{
    //    var userbag = await RedisManager.GetListByRange<UserBag>(_id + "bag");
    //    if(userbag==null)
    //    {
    //        userbag=await UserBag.SelectQueryAsync(_id);
    //        if (userbag == null)
    //            return false;
    //        _bagList = userbag.Select((v, i) => (value: v, index: i))
    //            .ToDictionary(pair => pair.index, pair => pair.value);
    //    }

    //    return true;
    //}
    
    
    
    private async Task<bool> GetUserTeam()
    {
        var userTeam = await BringTable<UserTeam>();
        if(userTeam==null)
        {
            userTeam=await UserTeam.SelectQueryOrDefaultAsync(_id);
            if (userTeam == null)
                return false;
            AddUserData<UserTeam>(userTeam);
        }

        return true;
    }
    
    public async ValueTask<ErrorCode> InitializeTeam(string teamName)
    {
        var tblTeam = TblTeam.Get(teamName);
        var result=ErrorCode.NONE;
        if (tblTeam == null)
        {
            result=ErrorCode.CREATE_FAIL;
            return result;
        }
        UserTeam userTeam = new UserTeam();
        Int64 Code = await userTeam.CountTeamFromDB(teamName);
        userTeam.id = tblTeam.Id;
        userTeam.userId = _id;
        userTeam.nickName = teamName+Code.ToString();
        result=await userTeam.InsertUserTeam();
        return result;
    }
    
  
    private async Task<T> GetRedisData<T>()
    {
        var result = await RedisManager.GetHashValue<T>(_id, nameof(T));
        if (result.GetValueOrNull() == null)
        {
            return default(T);
        }
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

    
    
    
     private async Task<T> BringTable<T>()where T:class
     {
         IUserData? data;

         if (_userDatas.TryGetValue(nameof(T), out data))
         {
             return (T)data;
         }
         
         var result = await GetRedisData<T>();
         if (result == null)
         {
             return result;
         }
         AddUserData<T>((IUserData)result);
         return result;
    
     }
     
     
     
     
     
     public T GetTable<T>() where T:class
     {
         IUserData data;
         if (false==_userDatas.TryGetValue(nameof(T), out data)) 
         {
             return null;
         }
         
         return (T)data;
     }
}