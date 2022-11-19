using Dapper;
using Microsoft.AspNetCore.Mvc;
using Server.Interface;
using Server.Model.User;
using Server.Model.ReqRes;
using Server.Services;



namespace Server.Controllers;


[ApiController]
[Route("[controller]")]
//이 컨트롤러에서는 처음 게임을 실행했을 때 보내줘야하는 유저 데이터 처리
public class SetUpTableController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IDBManager _database;
    private readonly IRedisDatabase _redis;

    public SetUpTableController(ILogger<SetUpTableController> logger,
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
        //여기서 캐시데이터 확인 -> Redis 
        //없으면 DB에서 데이터 가져오기
        // 최종적으로 Response에
        //Team정보, 메일정보, 출첵정보
        //user_data와 register_result로 결과값 넣어주기
        //출첵
        // 이전에 받은날짜와 현재가 1일 이상 차이나고 IsChecked==false 이면
        // 메일로 보내고 CheckDay++,RecvDate는 Today로 한다.
        //RecvDate -> lastAccessDate Team 테이블쪽으로 옮기기 
        SetUpResponse response = await gameDb.MakeSetUpResponse(req.ID);
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


        var date = DateTime.Today;
        string nickName = request.TeamName + '#' + request.ID;
        UserTeam userTeam = new UserTeam(TeamIdResult.Item2, request.ID, nickName);
        UserItem userItem = new UserItem() { ItemId = ItemIdResult.Item2, Quantity = 1, UserId = request.ID,Kind="item" };
        UserAttendance userAttendance = new UserAttendance(request.ID, "dailyCheckIn", date);
        await using (var connection = await gameDb.GetDBConnection())
        {
            var insertQuery = userTeam.InsertQuery();
            var affectRow = await connection.ExecuteAsync(insertQuery.Item1, insertQuery.Item2);
            if (affectRow == 0)
            {
                response.Result = ErrorCode.NOID;
            }

            insertQuery = userItem.InsertQuery();
            affectRow = await connection.ExecuteAsync(insertQuery.Item1, insertQuery.Item2);
            if (affectRow == 0)
            {
                response.Result = ErrorCode.NOID;
            }

            insertQuery = userAttendance.InsertQuery();
            affectRow = await connection.ExecuteAsync(insertQuery.Item1, insertQuery.Item2);
            if (affectRow == 0)
            {
                response.Result = ErrorCode.NOID;
            }
        }


        return response;
    }

    
}
