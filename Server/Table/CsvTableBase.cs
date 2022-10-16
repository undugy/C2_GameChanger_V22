using CsvHelper;

namespace Server.Table
{
    /// <summary>
    /// csv 기획 테이블 기본 클래스
    /// </summary>
    public abstract class CsvTableBase
    {
        public delegate bool Func();      //< Execute 함수에 쓰일 delegate 
        
        /// <summary>
        /// 해당 csv파일의 데이터를 모두 읽을 때까지 func delegate를 반복적으로 수행
        /// </summary>
        /// <param name="csv"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public bool Execute(CsvReader csv, Func func)
        {
            Int32 count = 0;
            
            while (csv.Read())
            {
                if (count == 0)
                {
                    csv.GetRecord<dynamic>();
                }

                if (!func())
                    return false;

                ++count;
            }
            return true;
        }

        /// <summary>
        /// 기획 데이터의 load 수행
        /// 관련 Table은 반드시 해당 형태의 함수가 존재해야 합니다
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool Load(CsvReader obj)
        {
            return false;
        }      
    }
}