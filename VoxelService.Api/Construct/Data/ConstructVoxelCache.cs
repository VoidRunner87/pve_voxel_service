using Microsoft.Extensions.Caching.Memory;

namespace VoxelService.Api.Construct.Data;

public static class ConstructVoxelCache
{
    public static MemoryCache Data { get; } = new(
        new MemoryCacheOptions
        {
            TrackStatistics = true,
            ExpirationScanFrequency = TimeSpan.FromSeconds(1 / 20d)
        }
    );

    public static void SetConstructData(ulong constructId, ConstructVoxelData data)
    {
        Data.Set(
            constructId, 
            data,
            GetDefaultMemoryCacheEntryOptions()
        );
    }

    public static void RefreshCache(ulong constructId)
    {
        Data.Set(
            constructId,
            Data.Get(constructId),
            GetDefaultMemoryCacheEntryOptions()
        );
    }

    public static ConstructVoxelData? Get(ulong constructId) => Data.Get<ConstructVoxelData>(constructId);

    private static MemoryCacheEntryOptions GetDefaultMemoryCacheEntryOptions()
    {
        return new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(10)
        };
    }
}