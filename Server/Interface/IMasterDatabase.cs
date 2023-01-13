using Server.Table;

namespace Server.Interface;

public interface IMasterDatabase:IDataBase
{
    public Task<Tuple<ErrorCode, uint>> SelectSingleItemId(string itemName);
    public Task<Tuple<ErrorCode, uint>> SelectSingleTeamId(string teamName);
    public Task<Tuple<ErrorCode, TblDailyCheckIn?>> SelectSingleDailyCheckIn(UInt32 day);
}