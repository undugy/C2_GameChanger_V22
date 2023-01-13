using ZLogger;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Server.Interface;
using Server.Model.User;
using Server.Model.ReqRes;


namespace Server.Controllers;


[ApiController]
[Route("[controller]")]
public class CreateAccountController:ControllerBase
{
    private readonly ILogger _logger;
    private readonly IGameDataBase _gameDataBase;
    public CreateAccountController(ILogger<CreateAccountController> logger,IGameDataBase gameDataBase)
    {
        _logger = logger;
        _gameDataBase = gameDataBase;

    }
    [HttpPost]
    public async Task<CreateAccountResponse> Post(CreateAccountRequset request)
    {
        var response = new CreateAccountResponse();
        _logger.ZLogInformation($"Start CreateAccount ID:{request.ID},PW{request.PW}");
        var saltValue = HashFunctions.SaltString();
        var hashedPassword = HashFunctions.MakeHashingPassWord(saltValue, request.PW);
        await using (var connection = await _gameDataBase.GetDBConnection())
        {
            var userInfo = new UserInfo()
                { Email = request.ID, SaltValue = saltValue, HashedPassword = hashedPassword };
            var insertQuery = userInfo.InsertQuery();
            var affectRow = await connection.ExecuteAsync(insertQuery.Item1, insertQuery.Item2);
            if (affectRow == 0)
            {
                response.Result = ErrorCode.ALREADY_EXIST;
            }

        }
        
        return response;
    }
}

