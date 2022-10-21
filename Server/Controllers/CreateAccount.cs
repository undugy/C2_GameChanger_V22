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
    public ErrorCode Result { get; set; }
}