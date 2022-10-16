using CsvHelper;

namespace Server.Table.CsvImpl;

public class TblDailyCheckInTable:CsvTableBase
{
    public Int16 Day;
    public string ItemName;
    public int Quantity;

     
    public override string ToString()
    {
        return "DailyCheckInTable";
    }

    public override bool Load(CsvReader obj)
    {
        return Execute(obj, () =>
        {
            Int32 idx = 0;
            var day = obj.GetField<Int16>(idx++);
            var itemName = obj.GetField<string>(idx++);
            var quantity = obj.GetField<int>(idx++);
            //if (itemName == null) return false;
           bool result= TableMap<int, TblDailyCheckInTable>.GetInstance.InsertRow(day, new TblDailyCheckInTable()
            {
                Day=day,
                ItemName = itemName,
                Quantity = quantity
            });
           
            return true;
        });
    }

    public static TableMap<int, TblDailyCheckInTable> Instance
    {
        get { return TableMap<int, TblDailyCheckInTable>.GetInstance; }
    }
    
    public static TblDailyCheckInTable Get(int day)
    {
        var row = Instance.Get(day);
        if (row != null)
        {
            return row;
        }
        return null;
    }
}

