using CsvHelper;
using Server.Interface;
using Server.Services;

namespace Server.Table;

public class TblLeague:IMasterTable
{
    public Int32 Id;


    public override string ToString()
    {
        return "LeagueTable";
    }

    public  bool Load()
    {
        Dictionary<string, TblLeague> tblDictionary = new Dictionary<string, TblLeague>();
      
        MemcacheManager.Set(ToString(), tblDictionary);
            
        return true;
    }

    public static TblLeague? Get(string name)
    {
        var row = MemcacheManager.Get<Dictionary<string, TblLeague>>("LeagueTable");
        TblLeague tblLeague;
        if (row != null)
        {
            row.TryGetValue(name, out tblLeague);
            return tblLeague;
        }

        return null;
    }
}