namespace Server.Interface
{
    /// <summary>
    /// 미들웨어용 인터페이스
    /// 대부분 1:1 매칭일텐데 굳이 쓸 필요가 있을까...
    /// </summary>
    public interface IMiddlewareEntity
    {
        void Write(object message);
        object Read();
    }
}