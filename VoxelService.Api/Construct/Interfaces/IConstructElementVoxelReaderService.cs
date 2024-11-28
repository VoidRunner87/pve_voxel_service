using VoxelService.Api.Construct.Data;

namespace VoxelService.Api.Construct.Interfaces;

public interface IConstructElementVoxelReaderService
{
    Task<ConstructElementVoxelsOutcome> QueryConstructElementsBoundingBoxes(ulong constructId);
}