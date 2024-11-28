using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using NQutils.Config;
using VoxelService.Api.Common;
using VoxelService.Api.Common.Logging;
using VoxelService.Api.Construct;
using VoxelService.Api.DU;
using VoxelService.Api.Mesh;
using VoxelService.Api.Threads;

Config.ReadYamlFileFromArgs("mod", args);

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<ThreadManagerWorker>();
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddLogging(log => log.SetupLog(logWebHostInfo: true));
builder.WebHost.ConfigureKestrel(options =>
{
    var port = EnvironmentVariableHelper.GetIntEnvironmentVarOrDefault("PORT", 5050);
    options.ListenAnyIP(port);
});
builder.Services.RegisterMeshServices();
builder.Services.RegisterConstructServices();
builder.Services.RegisterDualUniverseDependencies();

var app = builder.Build();
app.UseRouting();

app.MapControllers();

app.Run();