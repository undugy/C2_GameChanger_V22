using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Server.Interface;
using ZLogger;
namespace Server.MiddleWare;

public class CheckUserSessionMiddleWare
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;
    private readonly IRedisDatabase _redis;
    public CheckUserSessionMiddleWare(RequestDelegate next,ILogger<CheckUserSessionMiddleWare>logger,IRedisDatabase redis)
    {
        _next = next;
        _logger = logger;
        _redis = redis;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path != "/login" &&
            context.Request.Path != "/CreateAccount")
        {
            StreamReader bodyStream = new StreamReader(context.Request.Body, Encoding.UTF8);
            string body = bodyStream.ReadToEndAsync().Result;

            var obj = JsonConvert.DeserializeObject(body) as JObject;

            var userID = (string)obj["ID"];
            var accessToken = (string)obj["Token"];
            if (null==userID)
            {
                return;
            }

            if (string.IsNullOrEmpty(accessToken))
            {
                return;
            }
            //Redis 인증확인
            var redisToken = await _redis.GetStringValue<string>(userID);
            
            if (redisToken != accessToken)
            {
                _logger.ZLogInformation($"{accessToken} token and {redisToken} is not matched");
            }
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
        }


        await _next(context);
        
        
    }
}