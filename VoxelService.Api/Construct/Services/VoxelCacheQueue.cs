using System.Collections.Concurrent;
using SharpGLTF.Schema2;
using VoxelService.Api.Common;
using VoxelService.Api.Construct.Data;
using VoxelService.Api.Construct.Interfaces;
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
) : ThreadHandle(threadId, threadManager, cancellationToken)
{
    private readonly ILogger _logger = provider.CreateLogger<VoxelCacheQueue>();

    private readonly IConstructMeshDownloaderService _constructMeshDownloaderService =
        provider.GetRequiredService<IConstructMeshDownloaderService>();

    private readonly IConstructElementVoxelReaderService _constructElementVoxelReaderService =
        provider.GetRequiredService<IConstructElementVoxelReaderService>();

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

        MeshDownloadOutcome meshDownloadOutcome;
        ConstructElementVoxelsOutcome elementVoxelsOutcome;

        using (var _ = new TimeMeasure(_logger, "data download"))
        {
            var readElementVoxelsTask = _constructElementVoxelReaderService
                .QueryConstructElementsBoundingBoxes(constructId);
            var downloadMeshTask = _constructMeshDownloaderService
                .DownloadConstructMeshAsync(constructId, 2);

            await Task.WhenAll([readElementVoxelsTask, downloadMeshTask]);

            meshDownloadOutcome = await downloadMeshTask;
            elementVoxelsOutcome = await readElementVoxelsTask;

            if (!meshDownloadOutcome.Success)
            {
                ReportHeartbeat();
                Thread.Sleep(TickSleep);
                _logger.LogError("Failed to Download Mesh: {Message}", meshDownloadOutcome.Message);

                return;
            }
        }

        var model = ModelRoot.ReadGLB(meshDownloadOutcome.Stream);
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

            foreach (var voxel in elementVoxelsOutcome.Voxels)
            {
                voxels.Add(voxel);
            }

            ConstructVoxelCache.SetConstructData(constructId, new ConstructVoxelData { Voxels = voxels });
        }

        ReportHeartbeat();
        Thread.Sleep(TickSleep);
    }
}