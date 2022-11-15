using Server.Interface;
using Server.Services;
using Server.MiddleWare;
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

});



MemcacheManager.Init();


var app = builder.Build();

app.UseRouting();


app.UseCheckUserSessionMiddleWare();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();
RedisManager.Init(app.Configuration.GetSection("DBConnection")["Redis"]);
DBManager.Init(app.Configuration);
app.Run();
