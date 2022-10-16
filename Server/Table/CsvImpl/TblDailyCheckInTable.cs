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
            var day = obj.GetField<int>(idx++);
            var itemName = obj.GetField<string>(idx++);
            var quantity = obj.GetField<int>(idx++);
            if (itemName == null) return false;
            TableMap<int, TblDailyCheckInTable>.GetInstance.InsertRow(Day, new TblDailyCheckInTable()
            {
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
    
    public static (string ,int) Get(int day)
    {
        var row = Instance.Get(day);
        if (row != null)
        {
            return (row.ItemName,row.Quantity);
        }
        return ("",0);
    }
}

