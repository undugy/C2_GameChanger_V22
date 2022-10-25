using Microsoft.Extensions.Caching.Memory;
using Server.Services;
using Server.Table;
using Server.Table.CsvImpl;
using ZLogger;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// DateTime m = DateTime.Now;
// DateTime b=DateTime.Now.AddDays(14);
// var a = new TimeSpan(DateTime.Now.Ticks);
// var c = new TimeSpan(b.Ticks).TotalDays;
// DateTime n = m.AddDays((long)c);
// Console.WriteLine(m.ToString("yyyyMMddHHmss"));

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
CsvTableLoder.GetInstance.Load();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseRouting();



app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();
RedisManager.Init(builder.Configuration.GetSection("DBConnection")["Redis"]);
DBManager.Init(builder.Configuration.GetSection("DBConnection")["MySqlGame"]);
app.Run();