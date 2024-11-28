using System.Numerics;

namespace VoxelService.Data;

public class Voxel(int x, int y, int z)
{
    public int X { get; } = x;
    public int Y { get; } = y;
    public int Z { get; } = z;

    public override bool Equals(object? obj) => obj is Voxel v && v.X == X && v.Y == Y && v.Z == Z;
    public override int GetHashCode() => HashCode.Combine(X, Y, Z);
    public override string ToString() => $"Voxel({X}, {Y}, {Z})";

    public Vector3 ToVector3()
    {
        return new Vector3(X, Y, Z);
    }

    public static Voxel FromVector3(Vector3 point, float voxelSize)
    {
        return VoxelPool.Voxel(
            (int)(point.X / voxelSize),
            (int)(point.Y / voxelSize),
            (int)(point.Z / voxelSize)
        );
    }
    
    public static Voxel operator +(Voxel a, Voxel b)
    {
        return VoxelPool.Voxel(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }
    
    public static Voxel operator -(Voxel a, Voxel b)
    {
        return VoxelPool.Voxel(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }
}