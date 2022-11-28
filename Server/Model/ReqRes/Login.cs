namespace Server.Model.ReqRes;

public class LoginRequset
{
    public string ID { get; set; }
    public string PW { get; set; }
}

public class LoginResponse
{
    public UInt32 ID { get; set; }
    public string Token { get; set; }
    public ErrorCode Result { get; set; }
}
