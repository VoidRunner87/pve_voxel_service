using System.Collections.Concurrent;
using System.Numerics;
using VoxelService.Data;

namespace VoxelService.Services;

public static class VoxelCuller
{
    public static HashSet<Voxel> CullBlockedVoxels(
        HashSet<Voxel> voxels,
        Vector3 sourcePoint,
        Vector3 coneDirection,
        float coneAngle,
        float voxelSize)
    {
        var culledVoxels = new ConcurrentBag<Voxel>();

        // Normalize cone direction
        coneDirection = Vector3.Normalize(coneDirection);
        var coneCosAngle = (float)Math.Cos(coneAngle);

        Parallel.ForEach(voxels, voxel =>
        {
            var voxelCenter = new Vector3(
                voxel.X * voxelSize + voxelSize / 2,
                voxel.Y * voxelSize + voxelSize / 2,
                voxel.Z * voxelSize + voxelSize / 2
            );

            // Vector from source to voxel
            var toVoxel = Vector3.Normalize(voxelCenter - sourcePoint);

            // Check if voxel is inside the cone
            var dotProduct = Vector3.Dot(coneDirection, toVoxel);

            if (dotProduct >= coneCosAngle)
            {
                // Perform a line-of-sight test
                if (IsVisible(sourcePoint, voxelCenter, voxels, voxelSize))
                {
                    culledVoxels.Add(voxel);
                }
            }
        });

        return new HashSet<Voxel>(culledVoxels);
    }

    private static bool IsVisible(Vector3 source, Vector3 target, HashSet<Voxel> voxels, float voxelSize)
    {
        // Ray direction
        var direction = Vector3.Normalize(target - source);

        // Ray traversal through the grid
        var stepSize = voxelSize / 2;
        var currentPoint = source;

        while (Vector3.Distance(currentPoint, target) > stepSize)
        {
            currentPoint += direction * stepSize;

            // Convert currentPoint to voxel coordinates
            var currentVoxel = Voxel.FromVector3(currentPoint, voxelSize);

            // If the current voxel blocks the ray and is not the target voxel, return false
            if (voxels.Contains(currentVoxel) && !IsTargetVoxel(currentVoxel, target, voxelSize))
            {
                return false;
            }
        }

        return true;
    }
    
    private static bool IsTargetVoxel(Voxel voxel, Vector3 target, float voxelSize)
    {
        var targetVoxel = Voxel.FromVector3(target, voxelSize);
        
        return targetVoxel.Equals(voxel);
    }
}
