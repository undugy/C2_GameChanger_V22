using CsvHelper;

namespace Server.Table.CsvImpl
{    
    public class TblCommonDefine : CsvTableBase
    {
        public string value;
        
        public override string ToString()
        {
            return "CommonDefine";
        }

        public override bool Load(CsvReader obj)
        {
            return Execute(obj, () =>
            {
                Int32 idx = 0;
                var name = obj.GetField<string>(idx++);
                var value = obj.GetField<string>(idx++);
                
                TableMap<string, TblCommonDefine>.GetInstance.InsertRow(name, new TblCommonDefine()
                {
                    value = value
                });
                return true;
            });            
        }

        public static TableMap<string, TblCommonDefine> Instance
        {
            get { return TableMap<string, TblCommonDefine>.GetInstance; }
        }

        public static string Get(string name)
        {
            var row = Instance.Get(name);
            if (row != null)
            {
                return row.value;
            }
            return null;
        }
    }
}