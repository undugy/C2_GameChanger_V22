namespace Server
{
    /// <summary>
    /// 싱글턴 클래스
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> where T : class, new() 
    { 
        protected static object _instanceLock = new object();
        protected static volatile T _instance;
     
        public static T GetInstance 
        {
            get
            {
                lock (_instanceLock)
                {
                    if(null == _instance) 
                        _instance = new T();
                } 
                return _instance;
            } 
        } 
    }
}