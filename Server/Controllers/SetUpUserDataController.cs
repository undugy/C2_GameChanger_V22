using Dapper;
using Microsoft.AspNetCore.Mvc;
using Server.Interface;
using Server.Model.User;
using Server.Model.ReqRes;

using Server.Table;
using ZLogger;

namespace Server.Controllers;


[ApiController]
[Route("[controller]")]
//이 컨트롤러에서는 처음 게임을 실행했을 때 보내줘야하는 유저 데이터 처리
public class SetUpUserDataController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IGameDataBase _gameDatabase;
    private readonly IMasterDatabase _masterDatabase;
    private readonly IRedisDatabase _redis;

    public SetUpUserDataController(ILogger<SetUpUserDataController> logger,
        IGameDataBase gameDatabase,
        IMasterDatabase masterDatabase,
        IRedisDatabase redis)
    {
        _logger = logger;
        _gameDatabase = gameDatabase;
        _masterDatabase = masterDatabase;
        _redis = redis;
    }

    // 팀 정보,우편함,가방,카드정보(이거 일단 제외)
    [HttpPost]
    public async Task<SetUpResponse> Post(SetUpRequest req)
    {
        SetUpResponse response;
        var lastAccess = await _gameDatabase.SelectUserLastAccess(req.ID);
        var attendanceResult = await _gameDatabase.SelectSingleUserAttendance(req.ID, "dailyCheckIn");
       
        if (attendanceResult.Item1 == ErrorCode.NOT_INIT)
        {
            response= new SetUpResponse(){Result = ErrorCode.NOT_INIT};
            return response;
        }
        var nowDate = DateTime.Today;
        var userDataList = new Dictionary<string,object>();
       
        if ((nowDate - lastAccess.Item2).Days >= 1 && attendanceResult.Item2.IsChecked == false)
        {
            var userAttendance = attendanceResult.Item2;
            var dailyReward = await _redis.GetHashValue<uint, TblDailyCheckIn>("dailycheckinreward", userAttendance.CheckDay);
            var userMail = new UserMail() { ItemId = dailyReward.ItemId,Quantity = dailyReward.Quantity,
                ContentType = "dailycheckinreward",ReceiveDate = nowDate, UserId = req.ID };
          
            var query = userMail.InsertQuery();
            userDataList.Add(query.Item1,query.Item2);
            userAttendance.CheckDay++;
            query = userAttendance.UpdateQuery();
            userDataList.Add(query.Item1,query.Item2);

        }
        
        response = await _gameDatabase.MakeSetUpResponse(req.ID);
        response.Result = await _gameDatabase.UpdateUserLastAccess(req.ID, nowDate);

        await using (var connection = await _gameDatabase.GetDBConnection())
        {
            foreach (var data in userDataList.ToList())
            {
                
                var affectRow = await connection.ExecuteAsync(data.Key, data.Value);
                if (affectRow == 0)
                {
                    response.Result = ErrorCode.NOID;
                    _logger.LogWarning("Insert Failed ErrorCode:{0}",ErrorCode.NOID);
                }
            }
        }
        return response;
    }

    [HttpPost]
    [Route("InitializeTeam")]
    public async Task<InitializeTeamResponse> Post(InitializeTeamRequest request)
    {
        var response = new InitializeTeamResponse();
        _logger.ZLogInformation("ID:{"+request.ID+"}"+"NAME:{"+request.TeamName+"}");
        var ItemIdResult = await _masterDatabase.SelectSingleItemId("NAMECHANGETICKET");
        if (ItemIdResult.Item1 != ErrorCode.NONE)
        {
            response.Result = ItemIdResult.Item1;
            return response;
        }

        var TeamIdResult = await _masterDatabase.SelectSingleTeamId(request.TeamName);
        if (TeamIdResult.Item1 != ErrorCode.NONE)
        {
            response.Result = TeamIdResult.Item1;
            return response;
        }

      
       
        var date = DateTime.Today;
        string nickName = request.TeamName + '#' + request.ID;
        
        using (var connection = await _gameDatabase.GetDBConnection())
        {
            var team=new UserTeam(TeamIdResult.Item2, request.ID, nickName).InsertQuery();
            var item=new UserItem() { ItemId = ItemIdResult.Item2, Quantity = 1, UserId = request.ID,Kind="item" }.InsertQuery();
            var attend= new UserAttendance(request.ID, "dailyCheckIn").InsertQuery();
            var access=new UserAccess(request.ID,date,date).InsertQuery();
            await connection.ExecuteAsync(team.Item1, team.Item2);
            await connection.ExecuteAsync(item.Item1, item.Item2);
            await connection.ExecuteAsync(attend.Item1, attend.Item2);
            await connection.ExecuteAsync(access.Item1, access.Item2);
        }


        return response;
    }

    
    
}
