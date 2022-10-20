using Server.Services;
using Microsoft.AspNetCore.Mvc;
using ZLogger;
using Dapper;
using CloudStructures.Structures;
using Server;

namespace Server.Controllers;
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
        //TODO DB에 아이디 있는지 확인
        //토큰인증 후 없으면 DB에서 아이디 존재유무확인 있으면 비밀번호 맞는지확인 
        var response = new PkLoginResponse();
        response.Result = ErrorCode.NONE;
        using (var conn=await DBManager.GetDBConnection())
        {
            try
            {
                var row = await conn.QuerySingleOrDefaultAsync<UserTable>(
                    @"select id,pw,saltValue from user where ID=@ID", new { ID = request.id });
                if (row == null)
                {
                    response.Result = ErrorCode.NOID;
                    return response;
                }

                var HashPw = DBManager.MakeHashingPassWord(row.saltValue, request.pw);

                if (row.pw == HashPw)
                {
                    //토큰 등록
                    string token = RedisManager.AuthToken();
                    var idDefaultExpiry = TimeSpan.FromDays(1);
                    var redisId = new RedisString<string>(RedisManager.GetConnection(), request.id, idDefaultExpiry);
                    await redisId.SetAsync(token);
                    response.Token = token;
                    return response;
                }
                   
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
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

public class UserTable
{
    public string id { get; set; }
    public string pw { get; set; }
    public string saltValue { get; set; }
}