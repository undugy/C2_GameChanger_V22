using CsvHelper;
using Server.Interface;
using Server.Services;
namespace Server.Table;

public class TblTeam:IMasterTable
{
    public Int32 Id;
    public string Location;
    public string Korea;
     
    public override string ToString()
    {
        return "TeamTable";
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