using Microsoft.AspNetCore.Mvc;
using ZLogger;
using Dapper;
using Server.Interface;
using Server.Model.ReqRes;
using Server.Services;
using Server.Table;

namespace Server.Controllers;
[ApiController]
[Route("[controller]")]
public class LoginController:Controller
{
    private readonly ILogger _logger;
    private readonly IDBManager _database;
    private readonly IRedisDatabase _redis;
    public LoginController(ILogger<LoginController> logger,IDBManager database, IRedisDatabase redis)
    {
        _logger = logger;
        _database = database;
        _redis = redis;
    }

    [HttpPost]
    public async Task<LoginResponse> Post(LoginRequset request)
    {
        _logger.ZLogInformation($"[Request Login] ID:{request.ID}, PW:{request.PW}");
        var response = new LoginResponse();
        var database = _database.GetDatabase<GameDatabase>(DBNumber.GameDatabase);
        
        var userInfoQuery = await database.SelectSingleUserInfo(request.ID);
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
            
            if (await _redis.SetStringValue<string>(userInfo.UserId.ToString(), token))
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

