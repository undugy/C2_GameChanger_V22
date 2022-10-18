using Server.Services;
using CsvHelper;
namespace Server.Table.CsvImpl;

public class TblLeague:CsvTableBase
{
    public Int32 Id;


    public override string ToString()
    {
        return "LeagueTable";
    }

    public override bool Load(CsvReader obj)
    {
        Dictionary<string, TblLeague> tblDictionary = new Dictionary<string, TblLeague>();
        Execute(obj, () =>
        {
            Int32 idx = 0;
            var name = obj.GetField<string>(idx++);
            var id = obj.GetField<Int32>(idx++);
            bool result= tblDictionary.TryAdd(name, new TblLeague
            {
                Id=id
            });
            return true;
        });
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