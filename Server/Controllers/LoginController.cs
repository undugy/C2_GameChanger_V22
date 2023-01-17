using Microsoft.AspNetCore.Mvc;
using ZLogger;

using Server.Interface;
using Server.Model.ReqRes;


namespace Server.Controllers;
[ApiController]
[Route("[controller]")]
public class LoginController:ControllerBase
{
    private readonly ILogger _logger;
    private readonly IGameDataBase _gameDatabase;
    private readonly IRedisDatabase _redis;
    public LoginController(ILogger<LoginController> logger,
        IGameDataBase gameDatabase, 
        IRedisDatabase redis)
    {
        _logger = logger;
        _gameDatabase = gameDatabase;
        _redis = redis;
    }

    [HttpPost]
    public async Task<LoginResponse> Post(LoginRequset request)
    {
        _logger.ZLogInformation($"[Request Login] ID:{request.ID}, PW:{request.PW}");
        var response = new LoginResponse();
       
        
        var userInfoQuery = await _gameDatabase.SelectSingleUserInfo(request.ID);
        response.Result = userInfoQuery.Item1;
        
        var userInfo = userInfoQuery.Item2;
        if (userInfo == null)
        {
            response.Token = "";
            response.Result = ErrorCode.NOID;
            return response;
        }
        
        var HashPw = HashFunctions.MakeHashingPassWord(userInfo.SaltValue, request.PW);
        if (userInfo.HashedPassword == HashPw)
        {
            //토큰 등록
            string token = HashFunctions.AuthToken();
            
            if (await _redis.SetStringValue(userInfo.UserId.ToString(), token))
            {
                response.Token = token;
                response.ID = userInfo.UserId;
                return response;
            }
        }
        response.Result = ErrorCode.WRONG_PW;
        return response;
    }

}

