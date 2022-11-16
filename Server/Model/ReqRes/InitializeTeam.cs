namespace Server.Model.ReqRes;

public class InitializeTeamRequest
{
    public UInt32 ID { get; set; }
    public string TeamName { get; set; }
    public string Token { get; set; }
}

public class InitializeTeamResponse
{
    public InitializeTeamResponse()
    {
        Result = ErrorCode.NONE;
    }

    public ErrorCode Result { get; set; }
}
