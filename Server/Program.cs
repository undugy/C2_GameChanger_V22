using Server.Interface;
using Server.Services;
using Server.MiddleWare;
using ZLogger;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddTransient<IGameDataBase, GameDatabase>();
builder.Services.AddTransient<IMasterDatabase, MasterDatabase>();
builder.Services.AddSingleton<IRedisDatabase, RedisDatabase>();

builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConfiguration(builder.Configuration.GetSection("logging"));
    logging.AddZLoggerConsole();

});


var app = builder.Build();
RedisDatabase.Init(app.Configuration);
GameDatabase.Init(app.Configuration);
MasterDatabase.Init(app.Configuration);
app.UseRouting();


app.UseCheckUserSessionMiddleWare();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });


app.Run();
