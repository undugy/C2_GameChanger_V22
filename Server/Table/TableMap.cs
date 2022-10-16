namespace Server.Table
{
    /// <summary>
    /// 테이블 클래스의 실제 뼈대가 되는 클래스
    /// </summary>
    /// <typeparam name="T_KEY"></typeparam>
    /// <typeparam name="T_VAL"></typeparam>
    public class TableMap<T_KEY, T_VAL> : Singleton<TableMap<T_KEY, T_VAL>> 
        where T_VAL : class 
    {
        private Dictionary<T_KEY, T_VAL> data_;       ///< 해당 테이블의 데이터 컨테이너
       
        public TableMap()
        {
            data_ = new Dictionary<T_KEY, T_VAL>();
        }
        
        /// <summary>
        /// 테이블에 해당 열을 추가한다
        /// </summary>
        /// <param name="key"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public bool InsertRow(T_KEY key, T_VAL row)
        {
            return data_.TryAdd(key, row);
        }

        /// <summary>
        /// 데이터를 가지고 온다
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T_VAL Get(T_KEY key)
        {
            T_VAL val;
            if (!data_.TryGetValue(key, out val))
            {
                return null;
            }

            return val;
        }

        /// <summary>
        /// 해당 디렉토리의 pair 값들을 가지고 온다
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<T_KEY, T_VAL>> GetPairList()
        {
            return data_.ToList<KeyValuePair<T_KEY, T_VAL>>();
        }

        /// <summary>
        /// 해당 리렉토리의 key값들을 가지고 온다
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T_KEY> GetKeyEnumerable()
        {
            foreach (var row in data_)
            {
                yield return row.Key;
            }             
        }
        
        /// <summary>
        /// 데이터를 삭제한다
        /// </summary>
        public void Clear()
        {
            data_.Clear();
        }
    }
}