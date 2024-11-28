using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using VoxelService.Api.Common;
using VoxelService.Api.Mesh;
using VoxelService.Api.Threads;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<ThreadManagerWorker>();
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.RegisterMeshServices();
builder.WebHost.ConfigureKestrel(options =>
{
    var port = EnvironmentVariableHelper.GetIntEnvironmentVarOrDefault("PORT", 5050);
    options.ListenAnyIP(port);
});

var app = builder.Build();
app.UseRouting();

app.MapControllers();

app.Run();