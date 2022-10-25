using CloudStructures.Structures;
using Server.Interface;
using Server.Services;
using Server.Table;
using Server.Table.CsvImpl;

namespace Server.Model.User;

public class User
{
    private readonly string _id;
    private Dictionary<string,IUserData> _userDatas;
    //private Dictionary<int,UserBag> _bagList;
    private RedisBit _checkInLog;
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

        var tblItem = TblItem.Get("NAMECHANGETICKET");
        var bagProduct = new BagProduct() { itemId = tblItem.Id, userId = _id, kind = "item", quantity = 1 };
        await bagProduct.InsertOrUpdateBagProduct();
        //await userInfo.SaveDataToRedis();
     
        
        return result;
    }

    public async Task<bool> LoadUserData()
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

    public async Task<bool> SetUpUser()
    {
        var userInfo = await BringTable<UserInfo>();
        
        var userTeam = await BringTable<UserTeam>();

        // TODO CheckUserLogin() 여기서 전날 로그인 했는데 보상 안받았으면 주기
        if (!UpdateBall())
        {
            return false;
        }
        userInfo.lastLoginTime = DateTime.Now.ToLocalTime();

        return true;
    }

    private bool UpdateBall()
    {
        var userInfo = GetTable<UserInfo>();
        var userMaxBall = ConstantValue.BallMax + userInfo.level;
        if (userInfo.ball >=userMaxBall )
        {
            return true;
        }

        var nowTime = DateTime.Now;
        //var userTime = nowTime- userInfo.lastLoginTime;
        var ballAddTime = nowTime.Ticks - userInfo.lastBallAddTime.Ticks;
        var elapsed = new TimeSpan(ballAddTime);
        //var result = userTime - ballAddTime;
        int quo=(int)elapsed.TotalMinutes / 60;
        var remain = elapsed.TotalMinutes % 60;
        userInfo.ball += quo;
        if (userInfo.ball > userMaxBall)
        {
            userInfo.ball = userMaxBall;
        }
        var result= nowTime.AddMinutes(-remain);
        userInfo.lastBallAddTime = result.ToLocalTime();
        
        
        
        //if()
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
        var name = typeof(T).Name;
        return _userDatas.TryAdd(name, value);
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
         var name = typeof(T).Name;
         if (_userDatas.TryGetValue(name, out data))
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
         var name = typeof(T).Name;
         if (false==_userDatas.TryGetValue(name, out data)) 
         {
             return null;
         }
         
         return (T)data;
     }
}