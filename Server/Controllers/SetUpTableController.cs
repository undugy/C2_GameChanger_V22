using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Server;
using Server.Model;
using Server.Model.User;
using Server.Services;
using Server.Table.CsvImpl;
using JsonConverter = System.Text.Json.Serialization.JsonConverter;

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
        User user = new User(req.ID);
        UserBag userBag = new UserBag(req.ID);
        if (!await user.LoadUserData())
        {
            throw new Exception("유저정보 불러오기 실패");
        }

        if (!await userBag.SetUpBagAndMail())
        {
            throw new Exception("유저가방 불러오기 실패");
        }

        if (!await user.SetUpUser())
        {
            throw new Exception("유저세팅 실패");
        }

        var items = JsonConvert.SerializeObject(userBag.GetUserBag());
        //var mails = JsonConvert.SerializeObject(userBag.GetUserMail());
        response.Res.Add("UserInfo",user.GetTable<UserInfo>());
        response.Res.Add("UserBag",items);
        response.Res.Add("UserMail",userBag.GetUserMail());
        await user.UpdateUserDatas();
        return response;
    }

    [HttpPost]
    [Route("InitializeTeam")]
    public async Task<PkInitializeTeamResponse>Post(PkInitializeTeamRequest request)
    {
        var response = new PkInitializeTeamResponse();
        response.Result = ErrorCode.NONE;
        //var userTeam = await RedisManager.GetHashValue<UserTeam>(req.ID, nameof(UserTeam));
        User user = new User(request.ID);
        response.Result = await user.InitializeTeam(request.TeamName);
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