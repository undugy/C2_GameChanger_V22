namespace Server.Model.ReqRes;

public class CreateAccountRequset
{
    public string ID { get; set; }
    public string PW { get; set; }
  
}

public class CreateAccountResponse
{
    public CreateAccountResponse()
    {
        Result = ErrorCode.NONE;
    }
    public ErrorCode Result { get; set; }
}