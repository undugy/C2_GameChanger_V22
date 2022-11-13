using CloudStructures.Structures;
using ZLogger;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Server.Interface;
using Server.Model.User;
using Server.Services;
using Server.Table;

namespace Server.Controllers;


[ApiController]
[Route("[controller]")]
public class CreateAccount:Controller
{
    private readonly ILogger _logger;
    private readonly IDBManager _database;
    public CreateAccount(ILogger<CreateAccount> logger,IDBManager database)
    {
        _logger = logger;
        _database = database;
    }
    [HttpPost]
    public async Task<PkCreateAccountResponse> Post(PkCreateAccountRequest request)
    {
        var response = new PkCreateAccountResponse();
        _logger.ZLogInformation($"Start CreateAccount ID:{request.ID},PW{request.PW}");
        var saltValue = HashFunctions.SaltString();
        var hashedPassword = HashFunctions.MakeHashingPassWord(saltValue, request.PW);
        await using (var connection = await _database.GetDBConnection())
        {
            UserInfo userInfo = new UserInfo()
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

public class PkCreateAccountRequest
{
    public string ID { get; set; }
    public string PW { get; set; }
  
}

public class PkCreateAccountResponse
{
    public PkCreateAccountResponse()
    {
        Result = ErrorCode.NONE;
    }
    public ErrorCode Result { get; set; }
}