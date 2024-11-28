using System.Numerics;
using NQ;

namespace VoxelService.Api.Common.Extensions;

public static class VectorMathHelper
{
    public static Vector3 ToVector3(this Vec3 v)
    {
        return new Vector3((float)v.x, (float)v.y, (float)v.z);
    }
}