using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Server;
using Server.Model;
using Server.Services;
using Server.Table.CsvImpl;

namespace Server.Controllers;


[ApiController]
[Route("[controller]")]
//이 컨트롤러에서는 처음 게임을 실행했을 때 보내줘야하는 유저 데이터 처리
public class SetUpTableController:ControllerBase
{
    private readonly ILogger _logger;

    public SetUpTableController(ILogger<SetUpTableController> logger)
    {
        _logger = logger;
    }
    // 팀 정보,우편함,가방,카드정보(이거 일단 제외)
    [HttpPost]
    public async Task<PkSetUpResponse> Post(PkSetUpRequest req)
    {
        Console.WriteLine(req.ID);
        //여기서 캐시데이터 확인 -> Redis 
        //없으면 DB에서 데이터 가져오기
        // 최종적으로 Response에
        //user_data와 register_result로 결과값 넣어주기
        //후에 필터에서 json으로 컨버팅
        PkSetUpResponse response = new PkSetUpResponse();
        return response;
    }

    [HttpPost]
    [Route("InitializeTeam")]
    public async Task<PkInitializeTeamResponse>Post(PkInitializeTeamRequest request)
    {
        var response = new PkInitializeTeamResponse();
        response.Result = ErrorCode.NONE;
        //var userTeam = await RedisManager.GetHashValue<UserTeam>(req.ID, nameof(UserTeam));
        var tblTeam = TblTeam.Get(request.TeamName);
        if (tblTeam == null)
        {
            response.Result=ErrorCode.CREATE_FAIL;
            return response;
        }
        UserTeam userTeam = new UserTeam();
        Int64 Code = await userTeam.CountTeamFromDB(request.TeamName);
        userTeam.id = tblTeam.Id;
        userTeam.userId = request.ID;
        userTeam.nickName = request.TeamName+Code.ToString();
        userTeam.teamName = request.TeamName;
        bool result=await userTeam.InsertUserTeam();
        if (result == false)
        {
            response.Result=ErrorCode.CREATE_FAIL;
            return response;
        }
        Console.WriteLine(userTeam.nickName);
        //response.jsonTeam = JsonConvert.SerializeObject(userTeam);
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
    public string ID { get; set; }
    public string TeamName { get; set; }
    public string Token { get; set; }
}

public class PkInitializeTeamResponse
{
    
    public ErrorCode Result { get; set; }
}
public class PkSetUpResponse
{
    public Dictionary<string, object> Res=new Dictionary<string, object>();
}