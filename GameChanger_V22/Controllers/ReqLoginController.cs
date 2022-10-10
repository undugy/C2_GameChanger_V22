using GameChanger_V22.Services;
using Microsoft.AspNetCore.Mvc;
using ZLogger;
using Dapper;
using CloudStructures.Structures;
using GameChanger_V22;

namespace GameChanger_V22.Controllers;
[ApiController]
[Route("[controller]")]
public class ReqLoginController:ControllerBase
{
    private readonly ILogger _logger;

    public ReqLoginController(ILogger<ReqLoginController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<PkLoginResponse> Post(PkLoginRequest request)
    {
        _logger.ZLogInformation($"[Request Login] ID:{request.id}, PW:{request.pw}");
        var response = new PkLoginResponse();
        string tokenValue = RedisManager.AuthToken();
        response.Result = ErrorCode.NONE;
        var idDefaultExpiry = TimeSpan.FromDays(1);
        try
        {
            var redisId = new RedisString<string>(RedisManager.s_redisConn, request.id, idDefaultExpiry);
            await redisId.SetAsync(tokenValue);
        }
        catch (Exception e)
        {
            response.Result = ErrorCode.NOID;
            throw;
        }

        response.Token = tokenValue;
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

public class UserTable
{
    public string id { get; set; }
    public string pw { get; set; }
    public string nickName { get; set; }
    public string saltValue { get; set; }
}