using System.Text;
using CloudStructures.Structures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Server.Interface;
using Server.Services;
using ZLogger;
namespace Server.MiddleWare;

public class CheckUserSessionMiddleWare
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;
    private readonly IRedisManager _redis;
    public CheckUserSessionMiddleWare(RequestDelegate next,ILogger<CheckUserSessionMiddleWare>logger,IRedisManager redis)
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
            StreamReader bodystream = new StreamReader(context.Request.Body, Encoding.UTF8);
            string body = bodystream.ReadToEndAsync().Result;

            var obj = (JObject)JsonConvert.DeserializeObject(body);

            var userID = (UInt32)obj["ID"];
            var accessToken = (string)obj["Token"];
            if (null==userID)
            {
                return;
            }

            if (string.IsNullOrEmpty(accessToken))
            {
                return;
            }
            //TODO Redis 인증확인
            var RedisToken = await _redis.GetStringValue<string>(userID.ToString());
            
            if (RedisToken.ToString() != accessToken)
            {
                _logger.ZLogInformation($"{accessToken} token and {RedisToken.ToString()} is not matched");
            }
            
        }

        await _next(context);
        
        
    }
}