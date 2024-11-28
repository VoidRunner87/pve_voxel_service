namespace VoxelService.Api.Threads;

public interface IThreadHandle
{
    ThreadId ThreadId { get; }
    IThreadManager ThreadManager { get; }
    CancellationToken CancellationToken { get; }

    void ReportHeartbeat();

    Task Tick();
}