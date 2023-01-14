using Dapper;
using MySqlConnector;
using Server.Interface;
using Server.Table;

namespace Server.Services;

public class MasterDatabase:IMasterDatabase
{
    private static string _connectionString;
    public static void Init(IConfiguration configuration)
    {
        _connectionString= configuration.GetSection("DBConnection")["v22_master"];
        

    }
    
    
    public async Task<MySqlConnection>GetDBConnection()
    {
        
        return await GetOpenMySqlConnection(_connectionString);
    }
    
    public async Task<MySqlConnection> GetOpenMySqlConnection(string connectionString)
    {
        var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();
        return connection;
    }
    public string GetAllMasterTable()
    {
        return "SELECT * FROM item; " +
               "SELECT * FROM team;" +
               "SELECT * FROM league;" +
               "SELECT * FROM dailycheckinreward";
    }
    public async Task<Tuple<ErrorCode,uint>> SelectSingleItemId(string itemName)
    {
        ErrorCode errorCode = ErrorCode.NONE;
        uint itemId=0;
        await using (var connection= await GetDBConnection())
        {
            try
            {
                itemId = await
                    connection.QuerySingleOrDefaultAsync<uint>("SELECT ItemId FROM item WHERE Name=@name",
                        new { name = itemName });
            }
            catch (Exception e)
            {
                
                errorCode = ErrorCode.NOID;
            }
            
        }

        return new Tuple<ErrorCode,uint>(errorCode,itemId);
    }
    
    public async Task<Tuple<ErrorCode,uint>> SelectSingleTeamId(string teamName)
    {
        ErrorCode errorCode = ErrorCode.NONE;
        uint teamId = 0;
        await using (var connection= await GetDBConnection())
        {
            try
            {
                teamId = await
                    connection.QuerySingleOrDefaultAsync<uint>("SELECT TeamId FROM team WHERE Name=@name",
                        new { name = teamName });
            }
            catch (Exception e)
            {
                errorCode = ErrorCode.NOID;
            }
            
        }

        return new Tuple<ErrorCode,uint>(errorCode,teamId);
    }
    
    public async Task<Tuple<ErrorCode,TblDailyCheckIn?>> SelectSingleDailyCheckIn(UInt32 day)
    {
        ErrorCode errorCode = ErrorCode.NONE;
        TblDailyCheckIn? tblDailyCheckIn=null;
        await using (var connection= await GetDBConnection())
        {
            try
            {
               tblDailyCheckIn = await
                    connection.QuerySingleOrDefaultAsync<TblDailyCheckIn>("SELECT * FROM dailycheckinreward WHERE Day=@DAY",
                        new { DAY = day });
            }
            catch (Exception e)
            {
                errorCode = ErrorCode.NOID;
            }
            
        }

        return new Tuple<ErrorCode,TblDailyCheckIn?>(errorCode,tblDailyCheckIn);
    }
    
    
    public async Task<Tuple<ErrorCode, IEnumerable<TblItem>>> SelectAllItem()
    {
        await using (var connection = await GetDBConnection())
        {
            var a = await connection.QueryAsync<TblItem>("SELECT * FROM item");
            var res = new Tuple<ErrorCode, IEnumerable<TblItem>>(ErrorCode.NONE, a);
            return res;
        }
    }
}