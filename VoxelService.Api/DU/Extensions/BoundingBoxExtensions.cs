using VoxelService.Data;
using BoundingBox = NQ.BoundingBox;

namespace VoxelService.Api.DU.Extensions;

public static class BoundingBoxExtensions
{
    public static List<Voxel> ToVoxels(this BoundingBox boundingBox, float voxelSize)
    {
        if (voxelSize <= 0)
            throw new ArgumentException("Voxel size must be greater than zero.", nameof(voxelSize));

        var voxels = new List<Voxel>();

        // Calculate the minimum and maximum voxel indices based on the voxel size
        var minX = (int)(boundingBox.min.x / voxelSize);
        var minY = (int)(boundingBox.min.y / voxelSize);
        var minZ = (int)(boundingBox.min.z / voxelSize);

        var maxX = (int)(boundingBox.max.x / voxelSize);
        var maxY = (int)(boundingBox.max.y / voxelSize);
        var maxZ = (int)(boundingBox.max.z / voxelSize);

        // Iterate over the voxel grid
        for (var x = minX; x <= maxX; x++)
        {
            for (var y = minY; y <= maxY; y++)
            {
                for (var z = minZ; z <= maxZ; z++)
                {
                    // Convert voxel indices back to world coordinates
                    voxels.Add(VoxelPool.Voxel(x, y, z));
                }
            }
        }

        return voxels;
    }
}