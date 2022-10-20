using System.Text;
using CloudStructures.Structures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Server.Services;
using ZLogger;
namespace Server.MiddleWare;

public class CheckUserSessionMiddleWare
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;
    public CheckUserSessionMiddleWare(RequestDelegate next,ILogger<CheckUserSessionMiddleWare>logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path != "/login" &&
            context.Request.Path != "/CreateAccount")
        {
            StreamReader bodystream = new StreamReader(context.Request.Body, Encoding.UTF8);
            string body = bodystream.ReadToEndAsync().Result;

            var obj = (JObject)JsonConvert.DeserializeObject(body);

            var userID = (string)obj["ID"];
            var accessToken = (string)obj["Token"];
            if (string.IsNullOrEmpty(userID))
            {
                return;
            }

            if (string.IsNullOrEmpty(accessToken))
            {
                return;
            }
            //TODO Redis 인증확인
            var RedisToken = await RedisManager.GetStringValue<string>(userID);
            if (RedisToken.ToString() != accessToken)
            {
                _logger.ZLogInformation($"{accessToken} token and {RedisToken.ToString()} is not matched");
            }
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
        }

        await _next(context);
        
        
    }
}