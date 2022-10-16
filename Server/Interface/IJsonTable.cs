using Newtonsoft.Json.Linq;

namespace Server.Interface
{
    /// <summary>
    /// json테이블 인터페이스
    /// </summary>
    public interface IJsonTable
    {
        bool Load(JObject obj);
    }
}