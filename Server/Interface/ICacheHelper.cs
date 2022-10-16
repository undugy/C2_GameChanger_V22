namespace Server.Interface
{
    /// <summary>
    /// 캐시 핼퍼 상속용 인터페이스
    /// </summary>
    public abstract class ICacheHelper : IDisposable
    {
        public virtual void Dispose() {}
    }
}