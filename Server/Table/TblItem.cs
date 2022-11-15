
using CsvHelper;
using Server.Interface;
using Server.Services;


namespace Server.Table;

public class TblItem:IMasterTable
{
    public Int32 Id;
    public string Name;

    public override string ToString()
    {
        return "ItemTable";
    }

    public  bool Load()
    {
        Dictionary<string, TblItem> tblDictionary = new Dictionary<string, TblItem>();
     
        MemcacheManager.Set(ToString(), tblDictionary);
            
        return true;
    }
    
    public static TblItem? Get(string name)
    {
        var row = MemcacheManager.Get<Dictionary<string, TblItem>>("ItemTable");
        TblItem tblItem;
        if (row != null)
        {
            row.TryGetValue(name,out tblItem);
            return tblItem;
        }
        return null;
    }

}