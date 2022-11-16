using ZLogger;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Server.Interface;
using Server.Model.User;
using Server.Model.ReqRes;
using Server.Services;
using Server.Table;

namespace Server.Controllers;


[ApiController]
[Route("[controller]")]
public class CreateAccountController:Controller
{
    private readonly ILogger _logger;
    private readonly IDBManager _database;
    public CreateAccountController(ILogger<CreateAccountController> logger,IDBManager database)
    {
        _logger = logger;
        _database = database;
    }
    [HttpPost]
    public async Task<CreateAccountResponse> Post(CreateAccountRequset request)
    {
        var response = new CreateAccountResponse();
        _logger.ZLogInformation($"Start CreateAccount ID:{request.ID},PW{request.PW}");
        var database = _database.GetDatabase<GameDatabase>(DBNumber.GameDatabase);
        var saltValue = HashFunctions.SaltString();
        var hashedPassword = HashFunctions.MakeHashingPassWord(saltValue, request.PW);
        await using (var connection = await database.GetDBConnection())
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

