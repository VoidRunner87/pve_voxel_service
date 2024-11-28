using System.Numerics;

namespace VoxelService.Data;

public class Voxel_2
{
    private readonly long _packed;

    private Voxel_2(int x, int y, int z)
    {
        // Pack the x, y, and z coordinates into a single long
        _packed = Pack(x, y, z);
    }

    // Getter properties unpack the coordinates from _packed
    public int X => (int)(_packed >> 42) & 0x1FFFFF; // Extract the top 21 bits
    public int Y => (int)(_packed >> 21) & 0x1FFFFF; // Extract the middle 21 bits
    public int Z => (int)_packed & 0x1FFFFF; // Extract the bottom 21 bits

    public override bool Equals(object? obj) => obj is Voxel_2 v && _packed == v._packed;
    public override int GetHashCode() => _packed.GetHashCode();
    public override string ToString() => $"Voxel({X}, {Y}, {Z})";

    public Vector3 ToVector3()
    {
        return new Vector3(X, Y, Z);
    }

    public static Voxel_2 Create(int x, int y, int z) => new(x, y, z);

    public static Voxel FromVector3(Vector3 point, float voxelSize)
    {
        return VoxelPool.Voxel(
            (int)(point.X / voxelSize),
            (int)(point.Y / voxelSize),
            (int)(point.Z / voxelSize)
        );
    }

    public static Voxel operator +(Voxel_2 a, Voxel_2 b)
    {
        return VoxelPool.Voxel(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }

    public static Voxel operator -(Voxel_2 a, Voxel_2 b)
    {
        return VoxelPool.Voxel(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }

    // Packing and unpacking methods
    private static long Pack(int x, int y, int z)
    {
        // Ensure values fit within 21 bits (-2^20 to 2^20-1)
        const int maxValue = 0x1FFFFF; // 21 bits
        if (x < -maxValue || x > maxValue || y < -maxValue || y > maxValue || z < -maxValue || z > maxValue)
        {
            throw new ArgumentOutOfRangeException($"Values must fit in 21 bits: x={x}, y={y}, z={z}");
        }

        // Shift and combine x, y, and z into a single long
#pragma warning disable CS0675 // Bitwise-or operator used on a sign-extended operand
        return ((long)(x & maxValue) << 42) | ((long)(y & maxValue) << 21) | (z & maxValue);
#pragma warning restore CS0675 // Bitwise-or operator used on a sign-extended operand
    }
}