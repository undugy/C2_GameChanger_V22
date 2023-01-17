using Server.Model.ReqRes;
using Server.Model.User;

namespace Server.Interface;

public interface IGameDataBase:IDataBase
{
    public Task<Tuple<ErrorCode, UserInfo?>> SelectSingleUserInfo(string email);
    public Task<Tuple<ErrorCode, DateTime>> SelectUserLastAccess(UInt32 userId);
    public Task<Tuple<ErrorCode, UserAttendance>> SelectSingleUserAttendance(UInt32 userId, string contentType);
    public Task<ErrorCode> UpdateUserLastAccess(UInt32 userId, DateTime lastAccess);
    public Task<SetUpResponse> MakeSetUpResponse(UInt32 userId);
    public Task<CheckInResponse> MakeCheckInResponse(UInt32 id, UInt32 itemId, uint quantity, string itemName);
    public Task<MailListResponse> GetMailList(MailListRequest request);
    public Task<UserMail?> SelectMail(UInt32 mailId);
    public Task<ErrorCode> DeleteMail(UInt32 mailId);
    public Task<ErrorCode> DeleteAllMail(UInt32 userId);
    public Task<ErrorCode> ReceiveByItemId(UserMail userMail, UInt32 userId);
    public Task<ErrorCode> ReceiveByItemName(UserMail userMail, UInt32 userId, string wealthName);
    public Task<IEnumerable<UserMail>?> SelectAllMail(UInt32 userID);
    public string CheckItemKind(UInt32 itemId);
}