
using CsvHelper;
using Server.Interface;
using Server.Services;


namespace Server.Table;

public class TblItem:IMasterTable
{
    public UInt32 ItemId { get; set; }
    public string Name { get; set; }

    public string GetRedisKey()
    {
        return "master:item";
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