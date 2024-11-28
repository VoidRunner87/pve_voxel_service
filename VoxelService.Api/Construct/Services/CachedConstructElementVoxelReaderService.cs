using Microsoft.Extensions.Caching.Memory;
using VoxelService.Api.Construct.Data;
using VoxelService.Api.Construct.Interfaces;
using VoxelService.Data;

namespace VoxelService.Api.Construct.Services;

public class CachedConstructElementVoxelReaderService(
    IConstructElementVoxelReaderService service
) : IConstructElementVoxelReaderService
{
    private readonly MemoryCache _elementVoxelCache = new(new MemoryCacheOptions());

    public async Task<ConstructElementVoxelsOutcome> QueryConstructElementsBoundingBoxes(ulong constructId)
    {
        try
        {
            var cachedVoxels = _elementVoxelCache.Get<HashSet<Voxel>>(constructId);

            if (cachedVoxels == null)
            {
                var outcome = await service.QueryConstructElementsBoundingBoxes(constructId);
                if (!outcome.Success)
                {
                    return ConstructElementVoxelsOutcome.UnknownFailure();
                }
                
                _elementVoxelCache.Set(
                    constructId,
                    outcome.Voxels,
                    new MemoryCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromHours(1)
                    }
                );

                return ConstructElementVoxelsOutcome.RetrievedVoxels(outcome.Voxels);
            }

            return ConstructElementVoxelsOutcome.RetrievedCachedVoxels(cachedVoxels);
        }
        catch (Exception e)
        {
            return ConstructElementVoxelsOutcome.Failed(e);
        }
    }
}