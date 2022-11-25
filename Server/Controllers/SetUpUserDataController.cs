using System.Collections;
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
//이 컨트롤러에서는 처음 게임을 실행했을 때 보내줘야하는 유저 데이터 처리
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

    // 팀 정보,우편함,가방,카드정보(이거 일단 제외)
    [HttpPost]
    public async Task<SetUpResponse> Post(SetUpRequest req)
    {
        var gameDb = _database.GetDatabase<GameDatabase>(DBNumber.GameDatabase);
        var lastAccess = await gameDb.SelectUserLastAccess(req.ID);
        var attendanceResult = await gameDb.SelectSingleUserAttendance(req.ID, "dailyCheckIn");
        var nowDate = DateTime.Today;
        var userDataList = new Dictionary<string,object>();
        //DB에서 데이터 가져오기
        // 최종적으로 Response에
        //Team정보, 메일정보, 출첵정보
        //출첵
        // 이전에 받은날짜와 현재가 1일 이상 차이나고 IsChecked==false 이면
        // 메일로 보내고 CheckDay++,RecvDate는 Today로 한다.
        if ((nowDate - lastAccess.Item2).Days >= 1 && attendanceResult.Item2.IsChecked == false)
        {
            var userAttendance = attendanceResult.Item2;
            var dailyReward = await _redis.GetHashValue<uint, TblDailyCheckIn>("dailycheckinreward", userAttendance.CheckDay);
            var userMail = new UserMail() { ItemId = dailyReward.ItemId,Quantity = dailyReward.Quantity,
                ContentType = "dailycheckinreward",ReceiveDate = nowDate, UserId = req.ID };
            var query = userMail.InsertQuery();
            //여기서 메일로 쏴준다.
            userDataList.Add(query.Item1,query.Item2);
            //checkday늘려주기
            userAttendance.CheckDay++;
            query = userAttendance.UpdateQuery();
            userDataList.Add(query.Item1,query.Item2);

        }
        
        SetUpResponse response = await gameDb.MakeSetUpResponse(req.ID);
        response.Result = await gameDb.UpdateUserLastAccess(req.ID, nowDate);
        //여기서 last Access 업데이트 
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
        //나중에 StoredProcedure로 바꿔도 좋을 것 같다.
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
