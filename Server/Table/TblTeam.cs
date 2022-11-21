using CsvHelper;
using Server.Interface;
using Server.Services;
namespace Server.Table;

public class TblTeam:IMasterTable
{
    public Int32 TeamId { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
     
    public string GetRedisKey()
    {
        return "master:team";
    }

    public  bool Load()
    { 
        Dictionary<string, TblTeam> tblDictionary = new Dictionary<string, TblTeam>();
        
        return true;
    }
    
    public static TblTeam? Get(string name)
    {
        var row = MemcacheManager.Get<Dictionary<string, TblTeam>>("TeamTable");
        TblTeam tblTeam;
        if (row != null&&
            (true==row.TryGetValue(name,out tblTeam)))
        {
            return tblTeam;
        }
        return null;
    }
}