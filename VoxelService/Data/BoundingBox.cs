using System.Numerics;

namespace VoxelService.Data;

public class BoundingBox(Vector3 min, Vector3 max)
{
    public Vector3 Min { get; set; } = min;
    public Vector3 Max { get; set; } = max;
}