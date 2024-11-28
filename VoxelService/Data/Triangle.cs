using System.Numerics;

namespace VoxelService.Data;

public class Triangle(Vector3 v0, Vector3 v1, Vector3 v2)
{
    public Vector3 V0 { get; } = v0;
    public Vector3 V1 { get; } = v1;
    public Vector3 V2 { get; } = v2;

    public BoundingBox Bounds { get; } =
        new(Vector3.Min(v0, Vector3.Min(v1, v2)), Vector3.Max(v0, Vector3.Max(v1, v2)));
}