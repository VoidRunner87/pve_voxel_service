using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using SharpGLTF.Schema2;
using SharpGLTF.Validation;
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

    private readonly MemoryCache _constructThrottlingCache = new(new MemoryCacheOptions());

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

        using var logScope = _logger.BeginScope(new Dictionary<string, object>
        {
            { nameof(constructId), constructId }
        });

        var isThrottled = _constructThrottlingCache.Get(constructId) != null;
        if (isThrottled)
        {
            ReportHeartbeat();
            _logger.LogInformation("Throttled");
            
            return;
        }

        MeshDownloadOutcome meshDownloadOutcome;
        ConstructElementVoxelsOutcome elementVoxelsOutcome;

        using (var _ = new TimeMeasure(_logger, "data download"))
        {
            var readElementVoxelsTask = _constructElementVoxelReaderService
                .QueryConstructElementsBoundingBoxes(constructId);
            var downloadMeshTask = _constructMeshDownloaderService
                .DownloadConstructMeshAsync(constructId, ConfigurationReader.GetMeshDownloadLOD());

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

        if (meshDownloadOutcome.Stream == null)
        {
            _logger.LogError("Successful Outcome but without Stream. Unexpected.");
            throw new InvalidOperationException("Successful Outcome but without Stream. Unexpected.");
        }

        ModelRoot? model;
        try
        {
            model = ModelRoot.ReadGLB(meshDownloadOutcome.Stream);
            _logger.LogInformation("Read GLB");
        }
        catch (SchemaException e)
        {
            _logger.LogError(e, "Construct mesh is invalid");
            model = null;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error: Construct mesh is invalid");
            model = null;
        }

        using (var _ = new TimeMeasure(_logger, "voxelization"))
        {
            HashSet<Voxel> voxels = [];
            if (model != null)
            {
                voxels = Voxelizer.VoxelizeModel(
                    model,
                    new VoxelizerConfiguration
                    {
                        VoxelSize = ConfigurationReader.GetVoxelSize()
                    }
                );
            }

            foreach (var voxel in elementVoxelsOutcome.Voxels)
            {
                voxels.Add(voxel.Pooled());
            }
            
            _logger.LogInformation("Found {Count} Voxels", voxels.Count);

            ConstructVoxelCache.SetConstructData(constructId, new ConstructVoxelData { Voxels = voxels });
        }

        _constructThrottlingCache.Set(
            constructId, 
            true,
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ConfigurationReader.GetConstructMeshDownloadThrottleTime()
            }
        );

        ReportHeartbeat();
        Thread.Sleep(TickSleep);
    }
}