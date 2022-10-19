using CsvHelper;
using Server.Services;
namespace Server.Table.CsvImpl;

public class TblTeam:CsvTableBase
{
    public Int32 Id;
    public string Location;
    public string Korea;
     
    public override string ToString()
    {
        return "TeamTable";
    }

    public override bool Load(CsvReader obj)
    { 
        Dictionary<string, TblTeam> tblDictionary = new Dictionary<string, TblTeam>();
       
        bool result= Execute(obj, () =>
        {
            Int32 idx = 0;
            var name = obj.GetField<string>(idx++);
            var id = obj.GetField<Int32>(idx++);
            var location = obj.GetField<string>(idx++);
            var korea = obj.GetField<string>(idx++);
            bool result= tblDictionary.TryAdd(name, new TblTeam
            {
                Id = id,
                Location = location,
                Korea=korea
            });
            return true;
        });
        MemcacheManager.Set(ToString(), tblDictionary);
            
        return result;
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