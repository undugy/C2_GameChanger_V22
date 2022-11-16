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
    private readonly IRedisManager _redis;
    public LoginController(ILogger<LoginController> logger,IDBManager database, IRedisManager redis)
    {
        _logger = logger;
        _database = database;
        _redis = redis;
    }

    [HttpPost]
    public async Task<LoginResponse> Post(LoginRequset request)
    {
        _logger.ZLogInformation($"[Request Login] ID:{request.id}, PW:{request.pw}");
        //TODO DB에 아이디 있는지 확인
        //토큰인증 후 없으면 DB에서 아이디 존재유무확인 있으면 비밀번호 맞는지확인 
        var response = new LoginResponse();
        var database = _database.GetDatabase<GameDatabase>(DBNumber.GameDatabase);
        
        var userInfoQuery = await database.SelectSingleUserInfo(request.id);
        response.Result = userInfoQuery.Item1;
        
        var userInfo = userInfoQuery.Item2;
        if (userInfo == null)
        {
            response.Token = "";
            response.Result = ErrorCode.NOID;
            return response;
        }
        
        var HashPw = HashFunctions.MakeHashingPassWord(userInfo.SaltValue, request.pw);
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

