using System.Numerics;
using SharpGLTF.Schema2;
using VoxelService.Data;

namespace VoxelService.Services;

public static class Voxelizer
{
    public static HashSet<Voxel> VoxelizeModel(ModelRoot model, VoxelizerConfiguration configuration)
    {
        var triangles = MeshHelpers.ExtractTrianglesFromModel(model);

        return VoxelizeMeshTriangles2(triangles, configuration.VoxelSize);
    }
    
    public static HashSet<Voxel> VoxelizeMeshTriangles2(List<Triangle> triangles, float voxelSize)
    {
        var voxels = new HashSet<Voxel>();

        foreach (var triangle in triangles)
        {
            // Get the bounding box of the triangle
            var boundsMin = Vector3.Min(triangle.V0, Vector3.Min(triangle.V1, triangle.V2));
            var boundsMax = Vector3.Max(triangle.V0, Vector3.Max(triangle.V1, triangle.V2));

            // Convert bounding box to voxel grid coordinates
            var gridMin = WorldToVoxel(boundsMin, voxelSize);
            var gridMax = WorldToVoxel(boundsMax, voxelSize);

            // Iterate over all voxels in the bounding box
            for (var x = gridMin.X; x <= gridMax.X; x++)
            {
                for (var y = gridMin.Y; y <= gridMax.Y; y++)
                {
                    for (var z = gridMin.Z; z <= gridMax.Z; z++)
                    {
                        // var voxelCenter = VoxelToWorld(new Voxel(x, y, z), voxelSize);

                        // Add voxel (no detailed triangle-voxel intersection check here for simplicity)
                        voxels.Add(VoxelPool.Voxel(x, y, z));
                    }
                }
            }
        }

        return voxels;
    }

    private static HashSet<Voxel> VoxelizeMeshTriangles(List<Triangle> triangles, float voxelSize)
    {
        var voxels = new HashSet<Voxel>();

        foreach (var triangle in triangles)
        {
            // Get the bounding box of the triangle
            var boundsMin = Vector3.Min(triangle.V0, Vector3.Min(triangle.V1, triangle.V2));
            var boundsMax = Vector3.Max(triangle.V0, Vector3.Max(triangle.V1, triangle.V2));

            // Convert bounding box to voxel grid coordinates
            var gridMin = WorldToVoxel(boundsMin, voxelSize);
            var gridMax = WorldToVoxel(boundsMax, voxelSize);

            // Iterate over all voxels in the bounding box
            for (var x = gridMin.X; x <= gridMax.X; x++)
            {
                for (var y = gridMin.Y; y <= gridMax.Y; y++)
                {
                    for (var z = gridMin.Z; z <= gridMax.Z; z++)
                    {
                        var voxel = VoxelPool.Voxel(x, y, z);
                        var voxelCenter = VoxelToWorld(voxel, voxelSize);

                        // Check if the triangle intersects this voxel
                        if (TriangleIntersectsVoxel(triangle, voxelCenter, voxelSize))
                        {
                            voxels.Add(voxel);
                        }
                    }
                }
            }
        }

        return voxels;
    }

    private static bool TriangleIntersectsVoxel(Triangle triangle, Vector3 voxelCenter, float voxelSize)
    {
        var halfSize = voxelSize / 2;

        var voxelMin = voxelCenter - new Vector3(halfSize);
        var voxelMax = voxelCenter + new Vector3(halfSize);

        return TriangleIntersectsAabb(triangle, voxelMin, voxelMax);
    }

    private static bool TriangleIntersectsAabb(Triangle triangle, Vector3 boxMin, Vector3 boxMax)
    {
        if (PointInAabb(triangle.V0, boxMin, boxMax)) return true;
        if (PointInAabb(triangle.V1, boxMin, boxMax)) return true;
        if (PointInAabb(triangle.V2, boxMin, boxMax)) return true;

        return false;
    }

    private static bool PointInAabb(Vector3 point, Vector3 boxMin, Vector3 boxMax)
    {
        return point.X >= boxMin.X && point.X <= boxMax.X &&
               point.Y >= boxMin.Y && point.Y <= boxMax.Y &&
               point.Z >= boxMin.Z && point.Z <= boxMax.Z;
    }

    private static Voxel WorldToVoxel(Vector3 point, float voxelSize)
    {
        return VoxelPool.Voxel(
            (int)Math.Floor(point.X / voxelSize),
            (int)Math.Floor(point.Y / voxelSize),
            (int)Math.Floor(point.Z / voxelSize)
        );
    }

    private static Vector3 VoxelToWorld(Voxel voxel, float voxelSize)
    {
        return new Vector3(
            voxel.X * voxelSize + voxelSize / 2,
            voxel.Y * voxelSize + voxelSize / 2,
            voxel.Z * voxelSize + voxelSize / 2
        );
    }
}