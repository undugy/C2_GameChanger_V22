using Microsoft.Extensions.Caching.Memory;
using Server.Interface;
using Server.Services;
using Server.MiddleWare;
using StackExchange.Redis;
using ZLogger;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddTransient<IDBManager, DBManager>();
builder.Services.AddTransient<IRedisManager, RedisManager>();


builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConfiguration(builder.Configuration.GetSection("logging"));
    logging.AddZLoggerConsole();
    logging.AddZLoggerFile("DailyLog.log");
    logging.AddZLoggerRollingFile((dt, x) =>
        $"logs/{dt.ToLocalTime():yyyy-MM-dd}_{x:000}.log", x => x.ToLocalTime().Date, 1024);
});



MemcacheManager.Init();
//CsvTableLoder.Load();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
   // app.UseDeveloperExceptionPage();
}
app.UseRouting();


app.UseCheckUserSessionMiddleWare();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();
RedisManager.Init(app.Configuration.GetSection("DBConnection")["Redis"]);
DBManager.Init(app.Configuration);
app.Run();