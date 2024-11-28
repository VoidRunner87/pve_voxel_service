using System.Collections.Concurrent;
using SharpGLTF.Schema2;
using VoxelService.Api.Common;
using VoxelService.Api.Construct.Data;
using VoxelService.Api.Mesh.Interfaces;
using VoxelService.Api.Threads;
using VoxelService.Data;
using VoxelService.Services;

namespace VoxelService.Api.Construct.Services;

public class VoxelCacheQueue(
    ThreadId threadId,
    IThreadManager threadManager,
    IServiceProvider provider,
    CancellationToken cancellationToken
)
    : ThreadHandle(threadId, threadManager, cancellationToken)
{
    private readonly ILogger _logger = provider.CreateLogger<VoxelCacheQueue>();

    private readonly IConstructMeshDownloaderService _constructMeshDownloaderService =
        provider.GetRequiredService<IConstructMeshDownloaderService>();

    private TimeSpan TickSleep { get; set; } = TimeSpan.FromSeconds(1 / 60d);

    public static ConcurrentQueue<ulong> Queue { get; } = [];

    public override async Task Tick()
    {
        if (!Queue.TryDequeue(out var constructId))
        {
            ReportHeartbeat();
            Thread.Sleep(TickSleep);

            return;
        }

        MeshDownloadOutcome outcome;

        using (var _ = new TimeMeasure(_logger, "mesh download"))
        {
            outcome = await _constructMeshDownloaderService.DownloadConstructMeshAsync(constructId, 2);

            if (!outcome.Success)
            {
                ReportHeartbeat();
                Thread.Sleep(TickSleep);
                _logger.LogError("Failed to Download Mesh: {Message}", outcome.Message);

                return;
            }
        }

        var model = ModelRoot.ReadGLB(outcome.Stream);
        _logger.LogInformation("Read GLB");

        using (var _ = new TimeMeasure(_logger, "voxelization"))
        {
            var voxels = Voxelizer.VoxelizeModel(
                model,
                new VoxelizerConfiguration
                {
                    VoxelSize = ConfigurationReader.GetVoxelSize()
                }
            );
            
            ConstructVoxelCache.SetConstructData(constructId, new ConstructVoxelData { Voxels = voxels });
        }

        ReportHeartbeat();
        Thread.Sleep(TickSleep);
    }
}