using System.Globalization;
using CsvHelper;

using Server.Table.CsvImpl;

namespace Server.Table
{
    /// <summary>
    /// Csv 파일 로더 클래스
    /// Properties 디렉토리의 csv 파일 로드
    /// </summary>
    public class CsvTableLoder : Singleton<CsvTableLoder>
    {
        private Dictionary<string, CsvTableBase> regTblMap_;

        /// <summary>
        /// 생성자
        /// </summary>
        public CsvTableLoder()
        {
            regTblMap_ = new Dictionary<string, CsvTableBase>();
            Regist("Properties/csv/dailyCheckInTable.csv", new TblDailyCheckIn());
            //Regist("Properties/csv/itemTable.csv", new TblProtocolDefine());  
        }

        /// <summary>
        /// 테이블 등록
        /// </summary>
        /// <param name="tbl"></param>
        /// <returns></returns>
        public bool Regist(string filepath, CsvTableBase tbl)
        {
            if (filepath == null || tbl == null)
            {
                // 에러
             
                return false;
            }

            if (regTblMap_.ContainsKey(filepath))
            {
                // 에러
                
                return false;
            }

            regTblMap_[filepath] = tbl;

            return true;
        }

        /// <summary>
        /// 테이블 데이터 로드
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool Load()
        {
            string currentPos = "Empty";
            try
            {
                foreach (var pair in regTblMap_)
                {
                   
                    TextReader readFile = new StreamReader(pair.Key);
                    var pharse = new CsvParser(readFile,CultureInfo.InvariantCulture);
                    
                    var csv = new CsvReader(pharse);
                    if (csv == null)
                    {
                        
                        //throw new CustomException("failed to make CsvReader.");
                    }
                    currentPos = pair.Key;
                    pair.Value.Load(csv);
                }

                return true;
            }

            catch (Exception e)
            {
                Console.WriteLine(currentPos);
                Console.WriteLine(e);
            }

            return false;
        }
    }
}