using MySqlConnector;

namespace Server.Interface;

public interface IDBManager
{
    void Regist(DBNumber dbNumber, IDataBase database);
    public T GetDatabase<T>(DBNumber dbNumber)where T:class;
    
    
}

