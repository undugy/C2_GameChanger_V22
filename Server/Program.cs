using Server.Interface;
using Server.Services;
using Server.MiddleWare;
using ZLogger;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddTransient<IDBManager, DBManager>();
builder.Services.AddSingleton<IRedisDatabase, RedisDatabase>();

builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConfiguration(builder.Configuration.GetSection("logging"));
    logging.AddZLoggerConsole();

});

//builder.Services.AddHostedService<DailyEvent>();

MemcacheManager.Init();

var app = builder.Build();
DBManager.Init(app.Configuration);
RedisDatabase.Init(app.Configuration);
app.UseRouting();


app.UseCheckUserSessionMiddleWare();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

app.Run();
