using Server.Interface;


namespace Server.Services;

public class DBManager: IDBManager
{
    private Dictionary<DBNumber,IDataBase> _dataBases;
    private ILogger _logger;
    public static void Init(IConfiguration configuration)
    {
        GameDatabase.Init(configuration.GetSection("DBConnection")["v22"]);
        MasterDatabase.Init(configuration.GetSection("DBConnection")["v22_master"]);
        
    }

    public DBManager(ILogger<DBManager>logger)
    {
        _dataBases = new Dictionary<DBNumber, IDataBase>();
        _logger = logger;
        Regist(DBNumber.GameDatabase,new GameDatabase());
        Regist(DBNumber.MasterDatabase,new MasterDatabase());
    }
    
    
    public void Regist(DBNumber dbNumber, IDataBase database)
    {
        if (!_dataBases.TryAdd(dbNumber, database))
        {
            _dataBases[dbNumber] = database;
        }
        
    }

    public T GetDatabase<T>(DBNumber dbNumber) where T : class
    {
        IDataBase? dataBase;
        
        if (!_dataBases.TryGetValue(dbNumber, out dataBase))
        {
            return default(T);
        }

        return (T)dataBase;
    }
}