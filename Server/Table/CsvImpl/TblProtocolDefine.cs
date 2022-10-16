using CsvHelper;

namespace Server.Table.CsvImpl
{    
    public class TblProtocolDefine : CsvTableBase
    {
        public string value;
        
        public override string ToString()
        {
            return "ProtocolDefine";
        }

        public override bool Load(CsvReader obj)
        {
            return Execute(obj, () =>
            {
                Int32 idx = 0;
                var name = obj.GetField<string>(idx++);
                var value = obj.GetField<string>(idx++);
                
                TableMap<string, TblProtocolDefine>.GetInstance.InsertRow(name, new TblProtocolDefine()
                {
                    value = value
                });
                return true;
            });            
        }

        public static TableMap<string, TblProtocolDefine> Instance
        {
            get { return TableMap<string, TblProtocolDefine>.GetInstance; }
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