using Microsoft.AspNetCore.Mvc;

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
        //여기서 캐시데이터 확인 -> Redis 
        // 없으면 DB에서 데이터 가져오기
        // 최종적으로 Response에
        //user_data와 register_result로 결과값 넣어주기
        //후에 필터에서 json으로 컨버팅
        PkSetUpResponse response = new PkSetUpResponse();
        return response;
    }

}

public class PkSetUpRequest
{
    public string ID;
    public string Token;
}

public class PkSetUpResponse
{
    public Dictionary<string, object> Res;
}