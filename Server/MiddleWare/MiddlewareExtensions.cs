namespace Server.MiddleWare;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseCheckUserSessionMiddleWare(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CheckUserSessionMiddleWare>();
    } 
}