using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Server.Interface;
using Server.Model.ReqRes;
using Server.Services;
using Server.Table;
using ZLogger;

namespace Server.Controllers;
[ApiController]
[Route("[controller]")]
public class MailController:Controller
{
    private readonly ILogger _logger;
    private readonly IDBManager _database;
    private readonly IRedisDatabase _redis;
    public MailController(ILogger<MailController> logger,IDBManager database,IRedisDatabase redis)
    {
        _logger = logger;
        _database = database;
        _redis = redis;
    }

    [HttpPost]
    [Route("ListUp")]
    public async Task<MailListResponse> Post(MailListRequest request)
    {
        var gameDb = _database.GetDatabase<GameDatabase>(DBNumber.GameDatabase);
        return await gameDb.GetMailList(request);
    }
    
    
    
    [HttpPost]
    public async Task<MailResponse> Post(MailRequest request)
    {
        var gameDb = _database.GetDatabase<GameDatabase>(DBNumber.GameDatabase);
        var response = new MailResponse();
        var userMail = await gameDb.SelectMail(request.MailIndex);
        var result = ErrorCode.NONE;
        if (userMail == null)
        {
            response.Result = ErrorCode.CREATE_FAIL;
            return response;
        }

        if (gameDb.CheckItemKind(userMail.ItemId) == "wealth")
        {
            var wealth = await _redis.GetHashValue<uint, TblItem>("item", userMail.ItemId);
            result = await gameDb.ReceiveByItemName(userMail, request.ID, wealth.Name);
            if (result != ErrorCode.NONE)
            {
                response.Result = result;
                return response;
            }
        }
        else
        {
            result = await gameDb.ReceiveByItemId(userMail, request.ID);
            if (result != ErrorCode.NONE)
            {
                response.Result = result;
                return response;
            }
        }

        result = await gameDb.DeleteMail(request.MailIndex);
        if (result != ErrorCode.NONE)
        {
            response.Result = result;
            return response;
        }

        response.Quantity = userMail.Quantity;
        response.ItemId = userMail.ItemId;
        return response;
    }


    [HttpPost]
    [Route("ReceiveAll")]
    public async Task<ReceiveAllMailResponse> Post(ReceiveAllMailRequest request)
    {
        var gameDb = _database.GetDatabase<GameDatabase>(DBNumber.GameDatabase);
        var response = new ReceiveAllMailResponse();
        var result = ErrorCode.NONE;
        var userMails = await gameDb.SelectAllMail(request.ID);
        if (userMails == null)
        {
            response.Result = ErrorCode.CREATE_FAIL;
            return response;
        }

        foreach (var mail in userMails)
        {
            if (gameDb.CheckItemKind(mail.ItemId) == "wealth")
            {
                var wealth = await _redis.GetHashValue<uint, TblItem>("item", mail.ItemId);
                result = await gameDb.ReceiveByItemName(mail, request.ID, wealth.Name);
                if (result != ErrorCode.NONE)
                {
                    response.Result = result;
                    return response;
                }
            }
            else
            {
                result = await gameDb.ReceiveByItemId(mail, request.ID);
                if (result != ErrorCode.NONE)
                {
                    response.Result = result;
                    return response;
                }
            }
            response.ReceiveItemList.Add(mail.ItemId,mail.Quantity);
        }

        result = await gameDb.DeleteAllMail(request.ID);
        if (result != ErrorCode.NONE)
        {
            response.Result = result;
            return response;
        }

        return response;
    }
}