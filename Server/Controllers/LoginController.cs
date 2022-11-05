using Server.Services;
using Microsoft.AspNetCore.Mvc;
using ZLogger;
using Dapper;
using CloudStructures.Structures;
using Server;
using Server.Interface;
using Server.Model.User;
using StackExchange.Redis;
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
    public async Task<PkLoginResponse> Post(PkLoginRequest request)
    {
        _logger.ZLogInformation($"[Request Login] ID:{request.id}, PW:{request.pw}");
        //TODO DB에 아이디 있는지 확인
        //토큰인증 후 없으면 DB에서 아이디 존재유무확인 있으면 비밀번호 맞는지확인 
        var response = new PkLoginResponse();
        response.Result = ErrorCode.NONE;
        var userInfo = await UserInfo.SelectQueryOrDefaultAsync(request.id);
        if (userInfo == null)
        {
            response.Token = "";
            response.Result = ErrorCode.NOID;
            return response;
        }
        var HashPw = ConstantValue.MakeHashingPassWord(userInfo.saltValue, request.pw);
        if (userInfo.pw == HashPw)
        {
            //토큰 등록
            string token = ConstantValue.AuthToken();
            
            if (await _redis.SetStringValue<string>(request.id+"Login", token))
            {
                response.Token = token;
                //userInfo.lastLoginTime=DateTime.Now;
                return response;
            }
        }
        response.Result = ErrorCode.WRONG_PW;
        return response;
    
    }

}

public class PkLoginRequest
{
    public string id { get; set; }
    public string pw { get; set; }
}

public class PkLoginResponse
{
    public string Token { get; set; }
    public ErrorCode Result { get; set; }
}
