namespace Server.Model.ReqRes;

public class CheckInResponse
{
    public string RewardName { get; set; }
    public UInt32 RewardQuantity { get; set; }
    public ErrorCode Result { get; set; }
    public DateTime ReceiveDate { get; set; }
}

public class CheckInRequest
{
    public UInt32 ID { get; set; }
    public string Token { get; set; }
    public string ContentType { get; set; }

}