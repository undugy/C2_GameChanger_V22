using System.Runtime.InteropServices;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Server.Interface;
using Server.Model.User;
using Server.Services;
using StackExchange.Redis;


namespace Server.Controllers;


[ApiController]
[Route("[controller]")]
//이 컨트롤러에서는 처음 게임을 실행했을 때 보내줘야하는 유저 데이터 처리
public class SetUpTableController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IDBManager _database;
    private readonly IRedisManager _redis;

    public SetUpTableController(ILogger<SetUpTableController> logger,
        IDBManager database,
        IRedisManager redis)
    {
        _logger = logger;
        _database = database;
        _redis = redis;
    }

    // 팀 정보,우편함,가방,카드정보(이거 일단 제외)
    [HttpPost]
    public async Task<PkSetUpResponse> Post(PkSetUpRequest req)
    {

        //여기서 캐시데이터 확인 -> Redis 
        //없으면 DB에서 데이터 가져오기
        // 최종적으로 Response에
        //Team정보, 메일정보, 출첵정보
        //user_data와 register_result로 결과값 넣어주기
        PkSetUpResponse response = new PkSetUpResponse();

        return response;
    }

    [HttpPost]
    [Route("InitializeTeam")]
    public async Task<PkInitializeTeamResponse> Post(PkInitializeTeamRequest request)
    {
        var response = new PkInitializeTeamResponse();
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
            response.Result = ItemIdResult.Item1;
            return response;
        }



        string nickName = request.TeamName + '#' + request.ID;
        UserTeam userTeam = new UserTeam(TeamIdResult.Item2, request.ID, nickName);
        UserItem userItem = new UserItem() { ItemId = ItemIdResult.Item2, Quantity = 1, UserId = request.ID };

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


        }


        return response;
    }


}
    public class PkSetUpRequest
    {
        public string ID { get; set; }
        public string Token { get; set; }
    }


    public class PkInitializeTeamRequest
    {
        public UInt32 ID { get; set; }
        public string TeamName { get; set; }
        public string Token { get; set; }
    }

    public class PkInitializeTeamResponse
    {
        public PkInitializeTeamResponse()
        {
            Result = ErrorCode.NONE;
        }

        public ErrorCode Result { get; set; }
    }

    public class PkSetUpResponse
    {
        public Dictionary<string, object> Res = new Dictionary<string, object>();
    }
