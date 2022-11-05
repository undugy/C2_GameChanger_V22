using Microsoft.AspNetCore.Mvc;
using Server.Model.User;


namespace Server.Controllers;
[ApiController]
[Route("[controller]")]
public class CheckInController:Controller
{
    private readonly ILogger _logger;

    public CheckInController(ILogger<CheckInController> logger)
    {
        _logger = logger;
    }

    public async Task<PkCheckInResponse> Post(PkCheckInRequest request)
    {
        var response = new PkCheckInResponse();
        var user = new User(request.ID);
        var userBag = new UserBag(request.ID);
        BagProduct? bagProduct=null;
        if (!await user.LoadUserData())
        {
            throw new Exception("초기화 실패");
        }
        if(!await userBag.LoadUserBag())
        {
            throw new Exception("초기화 실패");
        }
        var userInfo = user.GetTable<UserInfo>();
        if (userInfo.dailyCheck == false)
        {
            bagProduct = await userBag.DirectCheckIn(userInfo.checkDay);
            if(bagProduct==null)
            {
                throw new Exception("아이템획득 실패");
                
            }
            userInfo.dailyCheck = true;
            await userInfo.SaveDataToRedis();
            await userInfo.SaveDataToDB();
        }


        if (bagProduct == null)
        {
            response.Result = ErrorCode.ALREADY_GET;
            return response;
        }
        response.RewardName = TblDailyCheckIn.Get(userInfo.checkDay).ItemName;
        response.RewardQuantity = bagProduct.quantity;
        response.ReceiveDate=DateTime.Now.ToLocalTime();
       
        return response;

    }
}

public class PkCheckInResponse
{
    public string RewardName { get; set; }
    public int RewardQuantity { get; set; }
    public ErrorCode Result { get; set; }
    public DateTime ReceiveDate { get; set; }
}

public class PkCheckInRequest
{
    public string ID { get; set; }
    public string Token { get; set; }
    
}