using Server.Model.User;

namespace Server.Model.ReqRes;
public class SetUpRequest
{
    public UInt32 ID { get; set; }
    public string Token { get; set; }
}



public class SetUpResponse
{
    public UserTeam TeamInfo { get; set; }
    public List<UserMail>MailList { get; set; }
    public ErrorCode Result = ErrorCode.NONE;
}
