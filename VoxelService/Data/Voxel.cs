using System.Numerics;

namespace VoxelService.Data;

public class Voxel(int x, int y, int z)
{
    public int X { get; } = x;
    public int Y { get; } = y;
    public int Z { get; } = z;

    protected bool Equals(Voxel other)
    {
        return X == other.X && Y == other.Y && Z == other.Z;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Voxel)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }

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