using Server.Model.User;

namespace Server.Model.ReqRes;

public class ReceiveAllMailRequest
{
    public UInt32 ID { get; set; }
    public string Token{ get; set; }
    
}

public class ReceiveAllMailResponse
{
    public Dictionary<UInt32,UInt32> ReceiveItemList { get; set; }
    public ErrorCode Result{ get; set; }

    public ReceiveAllMailResponse()
    {
        ReceiveItemList = new Dictionary<uint, uint>();
        Result = ErrorCode.NONE;
    }
}

public class MailRequest
{
    public UInt32 ID{ get; set; }
    public string Token{ get; set; }
    public UInt32 MailIndex { get; set; }
}

public class MailResponse
{
    public UInt32 ItemId { get; set; }
    public UInt32 Quantity { get; set; }
    public ErrorCode Result{ get; set; }
}

public class MailListRequest
{
    public UInt32 ID { get; set; }
    public string Token { get; set; }
    
}

public class MailListResponse
{
    public List<UserMail>MailList { get; set; }
    public ErrorCode Result{ get; set; }
}

