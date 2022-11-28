using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Server.Interface;
using Server.Model.ReqRes;
using Server.Services;

namespace Server.Controllers;
[ApiController]
[Route("[controller]")]
public class MailController:Controller
{
    private readonly ILogger _logger;
    private readonly IDBManager _database;

    public MailController(ILogger<MailController> logger,IDBManager database)
    {
        _logger = logger;
        _database = database;
    }

    [HttpPost]
    public async Task<MailListResponse> Post(MailListRequest request)
    {
        var gameDb = _database.GetDatabase<GameDatabase>(DBNumber.GameDatabase);
        return await gameDb.GetMailList(request);
    }
    
    
    
    [HttpPost]
    public async Task<MailResponse> Post(MailRequest request)
    {
        var gameDb = _database.GetDatabase<GameDatabase>(DBNumber.GameDatabase);
        return await gameDb.ReceiveMail(request);
    }


    [HttpPost]
    [Route("ReceiveAll")]
    public async Task<ReceiveAllMailResponse> Post(ReceiveAllMailRequest request)
    {
        var gameDb = _database.GetDatabase<GameDatabase>(DBNumber.GameDatabase);
        return await gameDb.ReceiveAllMail(request);
    }
}