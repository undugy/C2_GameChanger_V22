using CloudStructures.Structures;
using Server.Interface;
using ZLogger;

namespace Server.Services;

public class DailyEvent:IHostedService
{
    private readonly ILogger _logger;
    private readonly IRedisDatabase _redis;
    private Timer? _timer;
    public DailyEvent(ILogger<DailyEvent>logger,IRedisDatabase redis)
    {
        _logger = logger;
        _redis = redis;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        //TODO 정각 10분전으로 바꾸기
        TimeSpan interval=TimeSpan.FromHours(23);
        var nextRunTime = DateTime.Today.AddDays(1);
        var currTime = DateTime.Now;
        var firstIntervar = nextRunTime.Subtract(currTime);
        _timer = new Timer(AddDaily, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
       Action action = async () => 
       {
           var t1 = Task.Delay(firstIntervar);
           t1.Wait();
           AddDaily(null);
           _timer = new Timer(AddDaily, null, TimeSpan.Zero, interval);
       };
       Task.Run(action);
        
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    private async void AddDaily(object state)
    {
        _logger.ZLogInformation("추가중...");
        var date = DateTime.Today;
        var expiary = TimeSpan.FromDays(30);
        var redisBit = new RedisBit(_redis.GetConnection(), date.ToString(), expiary);
        await redisBit.SetAsync(0, false);
        var a = await redisBit.GetAsync(0);
        _logger.ZLogInformation(a.ToString());
    }
}