using Microsoft.Extensions.Caching.Memory;

namespace VoxelService.Data;

public static class VoxelPool
{
    private static readonly MemoryCache Cache = new(new MemoryCacheOptions { TrackStatistics = true });
    
    public static Voxel Voxel(int x, int y, int z)
    {
        var key = (x, y, z);

        return Cache.GetOrCreate(
            key,
            _ => new Voxel(x, y, z),
            new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(1)
            }
        )!;
    }

    public static long GetCount() => Cache.Count;
}