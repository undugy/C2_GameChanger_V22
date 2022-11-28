using CsvHelper;
using Server.Interface;
using Server.Services;
namespace Server.Table;

public class TblTeam
{
    public Int32 TeamId { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
  
}