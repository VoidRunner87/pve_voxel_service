using Microsoft.Extensions.Caching.Memory;

namespace VoxelService.Data;

public static class VoxelPool
{
    private static readonly MemoryCache Cache = new(
        new MemoryCacheOptions
        {
            TrackStatistics = true,
            ExpirationScanFrequency = TimeSpan.FromSeconds(1)
        });
    
    public static Voxel Voxel(int x, int y, int z)
    {
        return Cache.GetOrCreate(
            Data.Voxel.Pack(x, y, z),
            _ => new Voxel(x, y, z),
            new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(1)
            }
        )!;
    }

    public static long GetCount() => Cache.Count;
}