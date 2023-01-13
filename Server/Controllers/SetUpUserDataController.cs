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
public class SetUpUserDataController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IDBManager _database;
    private readonly IRedisDatabase _redis;

    public SetUpUserDataController(ILogger<SetUpUserDataController> logger,
        IDBManager database,
        IRedisDatabase redis)
    {
        _logger = logger;
        _database = database;
        _redis = redis;
    }
    
    [HttpPost]
    public async Task<SetUpResponse> Post(SetUpRequest req)
    {
        SetUpResponse response;
        var gameDb = _database.GetDatabase<GameDatabase>(DBNumber.GameDatabase);
        var lastAccess = await gameDb.SelectUserLastAccess(req.ID);
        var attendanceResult = await gameDb.SelectSingleUserAttendance(req.ID, "dailyCheckIn");
       
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
        
        response = await gameDb.MakeSetUpResponse(req.ID);
        response.Result = await gameDb.UpdateUserLastAccess(req.ID, nowDate);

        await using (var connection = await gameDb.GetDBConnection())
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
        var gameDb = _database.GetDatabase<GameDatabase>(DBNumber.GameDatabase);
        var masterDb = _database.GetDatabase<MasterDatabase>(DBNumber.MasterDatabase);


        var ItemIdResult = await masterDb.SelectSingleItemId("NAMECHANGETICKET");
        if (ItemIdResult.Item1 != ErrorCode.NONE)
        {
            response.Result = ItemIdResult.Item1;
            return response;
        }

        var TeamIdResult = await masterDb.SelectSingleTeamId(request.TeamName);
        if (TeamIdResult.Item1 != ErrorCode.NONE)
        {
            response.Result = TeamIdResult.Item1;
            return response;
        }

        var userDataList = new List<IUserData>();
 
        var date = DateTime.Today;
        string nickName = request.TeamName + '#' + request.ID;
        userDataList.Add(new UserTeam(TeamIdResult.Item2, request.ID, nickName));
        userDataList.Add(new UserItem() { ItemId = ItemIdResult.Item2, Quantity = 1, UserId = request.ID,Kind="item" });
        userDataList.Add( new UserAttendance(request.ID, "dailyCheckIn"));
        userDataList.Add(new UserLog(request.ID,date,date));
        await using (var connection = await gameDb.GetDBConnection())
        {
            foreach (var data in userDataList)
            {
                var insertQuery = data.InsertQuery();
                var affectRow = await connection.ExecuteAsync(insertQuery.Item1, insertQuery.Item2);
                if (affectRow == 0)
                {
                    response.Result = ErrorCode.NOID;
                    _logger.LogWarning("Insert Failed ErrorCode:{0}",ErrorCode.NOID);
                }
            }
        }


        return response;
    }

    
    
}
