using Microsoft.AspNetCore.Mvc;
using ZLogger;
using Dapper;
using Server.Interface;
using Server.Model.User;
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
        UserInfo? userInfo=null;
        await using (var connection= await _database.GetDBConnection())
        {
           
            try
            {
                userInfo = await connection.QuerySingleOrDefaultAsync<UserInfo>(
                    "SELECT * FROM user_info WHERE Email=@email",
                    new { email = request.id });
            }
            catch (Exception e)
            {
                _logger.ZLogDebug(e.Message);
                response.Result = ErrorCode.NOID;
            }
            
        }
        
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
            
            if (await _redis.SetStringValue<string>(request.id, token))
            {
                response.Token = token;
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
