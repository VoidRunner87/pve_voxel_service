using System.Collections.Concurrent;

namespace VoxelService.Data;

public static class VoxelPool
{
    private static readonly ConcurrentDictionary<(int, int, int), Voxel> Pool = new();
    
    public static Voxel Voxel(int x, int y, int z)
    {
        return Pool.GetOrAdd((x, y, z), key => Data.Voxel.Create(key.Item1, key.Item2, key.Item3));
    }
}