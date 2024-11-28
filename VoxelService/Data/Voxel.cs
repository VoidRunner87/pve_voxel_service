using System.Numerics;

namespace VoxelService.Data;

public class Voxel
{
    private Voxel(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public int X { get; }
    public int Y { get; }
    public int Z { get; }

    public override bool Equals(object? obj) => obj is Voxel v && v.X == X && v.Y == Y && v.Z == Z;
    public override int GetHashCode() => HashCode.Combine(X, Y, Z);
    public override string ToString() => $"Voxel({X}, {Y}, {Z})";

    public Vector3 ToVector3()
    {
        return new Vector3(X, Y, Z);
    }

    public static Voxel Create(int x, int y, int z) => new(x, y, z);

    public static Voxel FromVector3(Vector3 point, float voxelSize)
    {
        return VoxelPool.Voxel(
            (int)Math.Floor(point.X / voxelSize),
            (int)Math.Floor(point.Y / voxelSize),
            (int)Math.Floor(point.Z / voxelSize)
        );
    }
}