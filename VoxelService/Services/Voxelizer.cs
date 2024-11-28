using System.Numerics;
using SharpGLTF.Schema2;
using VoxelService.Data;
using VoxelService.Extensions;

namespace VoxelService.Services;

public static class Voxelizer
{
    public static HashSet<Voxel> VoxelizeModel(ModelRoot model, VoxelizerConfiguration configuration)
    {
        var triangles = MeshHelpers.ExtractTrianglesFromModel(model);

        return VoxelizeMeshTriangles(triangles, configuration.VoxelSize);
    }

    public static HashSet<Voxel> VoxelizeMeshTriangles(List<Triangle> triangles, float voxelSize)
    {
        var voxels = new HashSet<Voxel>();

        foreach (var triangle in triangles)
        {
            // Get the bounding box of the triangle
            var boundsMin = Vector3.Min(triangle.V0, Vector3.Min(triangle.V1, triangle.V2)).AsInt();
            var boundsMax = Vector3.Max(triangle.V0, Vector3.Max(triangle.V1, triangle.V2)).AsInt();

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
                        voxels.Add(new Voxel(x, y, z).Pooled());
                    }
                }
            }
        }

        return voxels;
    }

    private static Voxel WorldToVoxel(Vector3 point, float voxelSize)
    {
        return new Voxel(
            (int)(point.X / voxelSize),
            (int)(point.Y / voxelSize),
            (int)(point.Z / voxelSize)
        );
    }
}