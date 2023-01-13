using MySqlConnector;

namespace Server.Interface;

public interface IDBManager
{

    public T GetDatabase<T>(DBNumber dbNumber)where T:class;
    
    
}

