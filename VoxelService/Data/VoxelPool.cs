using System.Collections.Concurrent;

namespace VoxelService.Data;

public static class VoxelPool
{
    private static readonly ConcurrentDictionary<(int, int, int), Voxel> Pool = new();
    
    public static Voxel Voxel(int x, int y, int z)
    {
        var key = (x, y, z);
        
        // Add to the pool or get existing
        return Pool.GetOrAdd(key, static k => new Voxel(k.Item1, k.Item2, k.Item3));
    }

    public static long GetCount() => Pool.Count;
}