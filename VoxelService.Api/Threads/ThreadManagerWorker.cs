namespace VoxelService.Api.Threads;

public class ThreadManagerWorker(IServiceProvider provider) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return ThreadManager.GetInstance(provider, stoppingToken).Start();
    }
}