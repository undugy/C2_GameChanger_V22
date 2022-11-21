using Server.Interface;
using Server.Services;

namespace Server.Table;

public class TblDailyCheckIn:IMasterTable
{
    public UInt32 Day { get; set; }
    public UInt32 ItemId { get; set; }
    public UInt32 Quantity { get; set; }
    

    public string GetRedisKey()
    {
        return "master:dailycheckinreward";
    }

    public bool Load()
    { 
        Dictionary<int, TblDailyCheckIn> tblDictionary = new Dictionary<int, TblDailyCheckIn>();
       
        
        return true;
    }
    
    public static TblDailyCheckIn Get(int day)
    {
        var row = MemcacheManager.Get<Dictionary<int, TblDailyCheckIn>>("DailyCheckInTable");
        TblDailyCheckIn tblDailyCheckIn;
        if (row != null)
        {
            row.TryGetValue(day,out tblDailyCheckIn);
            return tblDailyCheckIn;
        }
        return null;
    }
    
    
}

