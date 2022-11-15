using Server.Interface;
using Server.Services;

namespace Server.Table;

public class TblDailyCheckIn:IMasterTable
{
    public UInt32 Id;
    public string ItemName;
    public int Quantity;
    
     
    public override string ToString()
    {
        return "DailyCheckInTable";
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

