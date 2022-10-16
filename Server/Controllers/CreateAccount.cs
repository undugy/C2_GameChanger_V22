using ZLogger;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Server.Services;

namespace Server.Controllers;


[ApiController]
[Route("[controller]")]
public class CreateAccount:Controller
{
    private readonly ILogger _logger;

    public CreateAccount(ILogger<CreateAccount> logger)
    {
        _logger = logger;
    }
    [HttpPost]
    public async Task<PkCreateAccountResponse> Post(PkCreateAccountRequest request)
    {
        var response = new PkCreateAccountResponse();
        _logger.ZLogInformation($"Start CreateAccount ID:{request.ID},PW{request.PW}");
        response.Result = ErrorCode.NONE;
        
        var saltValue = DBManager.SaltString();
        var hashedPw = DBManager.MakeHashingPassWord(saltValue, request.PW);
        try
        {
            using (var conn=await DBManager.GetDBConnection())
            {
                var row = await conn.ExecuteAsync(
                    @"INSERT user(id,pw,nickName,saltValue) Values(@id,@pw,@nickName,@salt) ",
                    new
                    {
                        id=request.ID,
                        pw=hashedPw,
                        nickName=request.NickName,
                        salt=saltValue
                    });
                if (row != 1)
                {
                    response.Result = ErrorCode.ALREADY_EXIST;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            response.Result = ErrorCode.CREATE_FAIL;
            return response;
        }

        return response;
    }
}

public class PkCreateAccountRequest
{
    public string ID { get; set; }
    public string PW { get; set; }
    public string NickName { get; set; }
}

public class PkCreateAccountResponse
{
    public ErrorCode Result { get; set; }
}