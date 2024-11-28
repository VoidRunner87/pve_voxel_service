using System.Collections.Concurrent;

namespace VoxelService.Api.Threads;

public static class LoopStats
{
    public static ConcurrentDictionary<string, DateTime> LastHeartbeatMap { get; } = new();
}