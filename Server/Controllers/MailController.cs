using Microsoft.AspNetCore.Mvc;
using Server.Interface;
using Server.Model.ReqRes;
using Server.Table;


namespace Server.Controllers;
[ApiController]
[Route("[controller]")]
public class MailController:ControllerBase
{
    private readonly ILogger _logger;
    private readonly IGameDataBase _gameDatabase;
    private readonly IRedisDatabase _redis;
    public MailController(ILogger<MailController> logger,IGameDataBase gameDatabase,IRedisDatabase redis)
    {
        _logger = logger;
        _gameDatabase = gameDatabase;
        _redis = redis;
    }

    [HttpPost]
    [Route("ListUp")]
    public async Task<MailListResponse> Post(MailListRequest request)
    {
        
        return await _gameDatabase.GetMailList(request);
    }
    
    
    
    [HttpPost]
    public async Task<MailResponse> Post(MailRequest request)
    {

        var response = new MailResponse();
        var userMail = await _gameDatabase.SelectMail(request.MailIndex);
        var result = ErrorCode.NONE;
        if (userMail == null)
        {
            response.Result = ErrorCode.CREATE_FAIL;
            return response;
        }

        if (_gameDatabase.CheckItemKind(userMail.ItemId) == "wealth")
        {
            var wealth = await _redis.GetHashValue<uint, TblItem>("item", userMail.ItemId);
            result = await _gameDatabase.ReceiveByItemName(userMail, request.ID, wealth.Name);
            if (result != ErrorCode.NONE)
            {
                response.Result = result;
                return response;
            }
        }
        else
        {
            result = await _gameDatabase.ReceiveByItemId(userMail, request.ID);
            if (result != ErrorCode.NONE)
            {
                response.Result = result;
                return response;
            }
        }

        result = await _gameDatabase.DeleteMail(request.MailIndex);
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
       
        var response = new ReceiveAllMailResponse();
        var result = ErrorCode.NONE;
        var userMails = await _gameDatabase.SelectAllMail(request.ID);
        if (userMails == null)
        {
            response.Result = ErrorCode.CREATE_FAIL;
            return response;
        }

        foreach (var mail in userMails)
        {
            if (_gameDatabase.CheckItemKind(mail.ItemId) == "wealth")
            {
                var wealth = await _redis.GetHashValue<uint, TblItem>("item", mail.ItemId);
                result = await _gameDatabase.ReceiveByItemName(mail, request.ID, wealth.Name);
                if (result != ErrorCode.NONE)
                {
                    response.Result = result;
                    return response;
                }
            }
            else
            {
                result = await _gameDatabase.ReceiveByItemId(mail, request.ID);
                if (result != ErrorCode.NONE)
                {
                    response.Result = result;
                    return response;
                }
            }
            response.ReceiveItemList.Add(mail.ItemId,mail.Quantity);
        }

        result = await _gameDatabase.DeleteAllMail(request.ID);
        if (result != ErrorCode.NONE)
        {
            response.Result = result;
            return response;
        }

        return response;
    }
}