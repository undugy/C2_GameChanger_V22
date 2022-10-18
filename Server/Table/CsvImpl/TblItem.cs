using CsvHelper;
using Server.Services;


namespace Server.Table.CsvImpl;

public class TblItem:CsvTableBase
{
    public Int32 Id;


    public override string ToString()
    {
        return "ItemTable";
    }

    public override bool Load(CsvReader obj)
    {
        Dictionary<string, TblItem> tblDictionary = new Dictionary<string, TblItem>();
        Execute(obj, () =>
        {
            Int32 idx = 0;
            var name = obj.GetField<string>(idx++);
            var id = obj.GetField<Int32>(idx++);
            bool result= tblDictionary.TryAdd(name, new TblItem
            {
                Id=id
            });
            return true;
        });
        MemcacheManager.Set(ToString(), tblDictionary);
            
        return true;
    }
    
    public static TblItem? Get(string name)
    {
        var row = MemcacheManager.Get<Dictionary<string, TblItem>>("ItemTable");
        TblItem tblItme;
        if (row != null)
        {
            row.TryGetValue(name,out tblItme);
            return tblItme;
        }
        return null;
    }

}