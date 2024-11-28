using VoxelService.Data;

namespace VoxelService.Api.Construct.Data;

public class ConstructVoxelData
{
    public required HashSet<Voxel> Voxels { get; set; }
}