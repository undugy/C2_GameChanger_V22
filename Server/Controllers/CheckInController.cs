using Dapper;
using Microsoft.AspNetCore.Mvc;
using Server.Interface;
using Server.Model.ReqRes;
using Server.Services;
using Server.Table;
using ZLogger;


namespace Server.Controllers;
[ApiController]
[Route("[controller]")]
public class CheckInController:Controller
{
    private readonly ILogger _logger;
    private readonly IDBManager _databaseManager;
    private readonly IRedisDatabase _redisDatabase;
    
    public CheckInController(ILogger<CheckInController> logger,
        IDBManager databaseManager,IRedisDatabase redisDatabase)
    {
        _logger = logger;
        _databaseManager = databaseManager;
        _redisDatabase = redisDatabase;
    }
    
    public async Task<PkCheckInResponse> Post(PkCheckInRequest request)
    {
        var response = new PkCheckInResponse();
        var gameDatabase = _databaseManager.GetDatabase<GameDatabase>(DBNumber.GameDatabase);
        var attendanceResult =await gameDatabase.SelectSingleUserAttendance(request.ID, request.ContentType);
        var logResult = await gameDatabase.SelectUserLastAccess(request.ID);
        if (attendanceResult.Item1 != ErrorCode.NONE)
        {
            _logger.ZLogDebug("출석체크 불러오기 실패");
        }
        
        var userAttendance = attendanceResult.Item2;
        var lastAccess = logResult.Item2;
        var date = DateTime.Now;
        // 접속날짜가 오늘이고 IsChecked가 false이면 
        if ((date - lastAccess).Days == 0 && userAttendance.IsChecked==false) 
        {
            var dailyReward = await _redisDatabase.GetHashValue<uint, TblDailyCheckIn>("dailycheckinreward", 
                userAttendance.CheckDay);
            var masterItem = await _redisDatabase.GetHashValue<uint, TblItem>("item", dailyReward.ItemId);
            userAttendance.IsChecked =true;
            response = await gameDatabase.MakeCheckInResponse(request.ID, dailyReward.ItemId, dailyReward.Quantity,
                masterItem.Name);
            response.ReceiveDate = date;
            response.Result = ErrorCode.NONE;
            
            userAttendance.CheckDay++;
            //업데이트 
        }

        
        await using (var connection = await gameDatabase.GetDBConnection())
        {
            var updateQuery = userAttendance.UpdateQuery();
            var result= await connection.ExecuteAsync(updateQuery.Item1, updateQuery.Item2);
        }
        
        return response;

    }
}

