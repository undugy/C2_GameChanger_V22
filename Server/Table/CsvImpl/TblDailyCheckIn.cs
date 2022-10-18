using System.ComponentModel.Design;
using CsvHelper;
using Microsoft.Extensions.Caching.Memory;
using Server.Services;

namespace Server.Table.CsvImpl;

public class TblDailyCheckIn:CsvTableBase
{
    
    public string ItemName;
    public int Quantity;
    
     
    public override string ToString()
    {
        return "DailyCheckInTable";
    }

    public override bool Load(CsvReader obj)
    { 
        Dictionary<int, TblDailyCheckIn> tblDictionary = new Dictionary<int, TblDailyCheckIn>();
       
        bool result= Execute(obj, () =>
        {
            Int32 idx = 0;
            var day = obj.GetField<Int16>(idx++);
            var itemName = obj.GetField<string>(idx++);
            var quantity = obj.GetField<int>(idx++);
            bool result= tblDictionary.TryAdd(day, new TblDailyCheckIn
            {
                ItemName = itemName,
               Quantity = quantity
            });
           return true;
        });
        MemcacheManager.Set(ToString(), tblDictionary);
            
        return result;
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

