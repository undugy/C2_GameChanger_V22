
using System.Collections;
using MySqlConnector;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using Server.Interface;
using Server.Model.User;
using StackExchange.Redis;
using ZLogger;

namespace Server.Services;

public class DBManager: IDBManager
{
    private Dictionary<DBNumber,IDataBase> _dataBases;
    public static void Init(IConfiguration configuration)
    {
        GameDatabase.Init(configuration.GetSection("DBConnection")["v22"]);
        MasterDatabase.Init(configuration.GetSection("DBConnection")["v22_master"]);
        
    }

    public DBManager()
    {
        _dataBases = new Dictionary<DBNumber, IDataBase>();
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
        IDataBase dataBase;
        if (!_dataBases.TryGetValue(dbNumber, out dataBase))
        {
            return default(T);
        }

        return (T)dataBase;
    }
}