using CloudStructures;
using CloudStructures.Structures;
using Server.Interface;
using Dapper;
using Server.Table;
using ZLogger;

namespace Server.Services;
public class RedisDatabase:IRedisDatabase
{
    private RedisConnection _redisConn;
    private const string _allowableCharacters = "abcdefghijklmnopqrstuvwxyz0123456789";

    //TODO 레디스의 Hash 자료구조를 이용해 유저정보 저장하기 

    public RedisConnection GetConnection() => _redisConn;
    private static RedisConfig _config;
    private readonly ILogger _logger;
    private readonly IDBManager _database;
    public static void Init(IConfiguration configuration)
    {
        _config = new RedisConfig("basic", configuration.GetSection("DBConnection")["Redis"]);
      
    }

    private async Task<ErrorCode> SetMasterTable<TKey, TVal>(string key, IEnumerable<KeyValuePair<TKey, TVal>> table)
    {
        var redisDict = GetHash<TKey, TVal>(key);
        await redisDict.SetAsync(table);
        return ErrorCode.NONE;
    }

    private async Task SetUpAllMasterData()
    {
        var masterDb = _database.GetDatabase<MasterDatabase>(DBNumber.MasterDatabase);
        using (var connection = await masterDb.GetDBConnection()) 
        {
            try
            {
                var multi = await connection.QueryMultipleAsync(masterDb.GetAllMasterTable());
                var items = multi.Read<TblItem>().ToDictionary(keySelector:m=>m.ItemId).AsEnumerable();
                var teams = multi.Read<TblTeam>().ToDictionary(keySelector:m=>m.TeamId).AsEnumerable();
                var leagues = multi.Read<TblLeague>().ToDictionary(keySelector:m=>m.LeagueId).AsEnumerable();
                var checkIn= multi.Read<TblDailyCheckIn>().ToDictionary(keySelector:m=>m.Day).AsEnumerable();
                multi.Dispose();
                await SetMasterTable("item", items);
                await SetMasterTable("team", teams);
                await SetMasterTable("league", leagues);
                await SetMasterTable("dailycheckinreward", checkIn);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }
    }
    
    
    public RedisDatabase(ILogger<RedisDatabase>logger,IDBManager database)
    {
        _redisConn = new RedisConnection(_config);
        _logger = logger;
        _database = database;
        _logger.ZLogInformation("Redis생성자 호출");
        var t = Task.Run(async () =>
        {
            await SetUpAllMasterData();
        });
        t.Wait();
    }
    
    
    public async Task<RedisResult<T>>GetHashValue<T>(string key,string subKey)
    {
        var redisId = new RedisDictionary<string,T>(GetConnection(),key,null);
        
        return await redisId.GetAsync(subKey);
    }
    public async Task<List<T>>GetListByRange<T>(string key)
    {
        var redisId = new RedisList<T>(GetConnection(),key,null);
        List<T> result = new List<T>();
        var redisList = await redisId.RangeAsync();
        result.AddRange(redisList.ToList());
        return result;
    }
    public  RedisDictionary<TKey,TVal>GetHash<TKey,TVal>(string key)
    {
        var redisId = new RedisDictionary<TKey,TVal>(GetConnection(),key,null);
        
        return redisId;
    }
    
    public async Task<RedisSortedSet<T>>GetSortedSet<T>(string key)
    {
        var redisId = new RedisSortedSet<T>(GetConnection(),key,null);
        await redisId.DeleteAsync();
        return redisId;
    }
    public async Task<T[]>GetSortedSetRangeByScore<T>(string key,int min,int max)
    {
        var redisId = new RedisSortedSet<T>(GetConnection(),key,null);
        await redisId.DeleteAsync();

        var result= await redisId.RangeByScoreAsync(min, max);
        
        return result;
    }
    public  async Task<RedisResult<T>> GetStringValue<T>(string key)
    {
        var redisId = new RedisString<T>(GetConnection(),key,null);
        
        return await redisId.GetAsync();
    }
    public  async Task<bool> SetStringValue<T>(string key, T value)
    {
        var defaultExpiry = TimeSpan.FromDays(1);
        var redisId = new RedisString<T>(GetConnection(),key,defaultExpiry);
        return await redisId.SetAsync(value);
    }
    public async Task<bool>SetHashValue<TKEY,T>(string key,TKEY subKey,T value)where T:class
    {
        var defaultExpiry = TimeSpan.FromDays(1);
        var redisId = new RedisDictionary<TKEY,T>(GetConnection(),key,defaultExpiry);
        var result= await redisId.SetAsync(subKey, value);
        return result;
    }
    public  async Task<long>InsertListValue<T>(string key,T value)where T:class
    {
        var defaultExpiry = TimeSpan.FromDays(1);
        var redisId = new RedisList<T>(GetConnection(),key,defaultExpiry);
        return await redisId.RightPushAsync(value);
    }
    
    public async Task<long>SetListValue<T>(string key,T value,TimeSpan?expiry=null)where T:class
    {
        var redisId = new RedisList<T>(GetConnection(),key,expiry);
        return await redisId.RightPushAsync(value);
    }
    
    
}

