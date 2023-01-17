using Dapper;
using Microsoft.AspNetCore.Mvc;
using Server.Interface;
using Server.Model.ReqRes;
using Server.Table;
using ZLogger;


namespace Server.Controllers;
[ApiController]
[Route("[controller]")]
public class CheckInController:ControllerBase
{
    private readonly ILogger _logger;
    private readonly IGameDataBase _gameDatabase;
    private readonly IRedisDatabase _redisDatabase;
    
    public CheckInController(ILogger<CheckInController> logger,
        IGameDataBase gameDatabase,IRedisDatabase redisDatabase)
    {
        _logger = logger;
        _gameDatabase = gameDatabase;
        _redisDatabase = redisDatabase;
    }
    
    public async Task<CheckInResponse> Post(CheckInRequest request)
    {
        var response = new CheckInResponse();
        var attendanceResult =await _gameDatabase.SelectSingleUserAttendance(request.ID, request.ContentType);
        var logResult = await _gameDatabase.SelectUserLastAccess(request.ID);
        if (attendanceResult.Item1 != ErrorCode.NONE)
        {
            _logger.ZLogDebug("출석체크 불러오기 실패");
        }
        
        var userAttendance = attendanceResult.Item2;
        var lastAccess = logResult.Item2;
        var date = DateTime.Now;

        if ((date - lastAccess).Days == 0 && userAttendance.IsChecked==false) 
        {
            var dailyReward = await _redisDatabase.GetHashValue<uint, TblDailyCheckIn>("dailycheckinreward", 
                userAttendance.CheckDay);
            var masterItem = await _redisDatabase.GetHashValue<uint, TblItem>("item", dailyReward.ItemId);
            userAttendance.IsChecked =true;
            response = await _gameDatabase.MakeCheckInResponse(request.ID, dailyReward.ItemId, dailyReward.Quantity,
                masterItem.Name);
            response.ReceiveDate = date;
            response.Result = ErrorCode.NONE;
            
            userAttendance.CheckDay++;
        }

        
        await using (var connection = await _gameDatabase.GetDBConnection())
        {
            var updateQuery = userAttendance.UpdateQuery();
            var result= await connection.ExecuteAsync(updateQuery.Item1, updateQuery.Item2);
        }
        
        return response;

    }
}

