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
        var masterDatabase = _databaseManager.GetDatabase<MasterDatabase>(DBNumber.MasterDatabase);
        var selectResult =await gameDatabase.SelectSingleUserAttendance(request.ID, request.ContentType);
        if (selectResult.Item1 != ErrorCode.NONE)
        {
            _logger.ZLogDebug("출석체크 불러오기 실패");
        }
        
        var userAttendance = selectResult.Item2;
        var date = DateTime.Now;
        // 접속날짜가 오늘이고 IsChecked가 false이면 
        if ((date - userAttendance.RecvDate).Days == 0 && userAttendance.IsChecked==false) 
        {
            var reward = masterDatabase.SelectSingleDailyCheckIn(userAttendance.CheckDay);
            userAttendance.IsChecked = true;
            userAttendance.RecvDate=date;
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

