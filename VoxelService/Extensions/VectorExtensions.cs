using System.Numerics;

namespace VoxelService.Extensions;

public static class VectorExtensions
{
    public static Vector3 AsInt(this Vector3 vector3)
    {
        return new Vector3((int)vector3.X, (int)vector3.Y, (int)vector3.Z);
    }
}