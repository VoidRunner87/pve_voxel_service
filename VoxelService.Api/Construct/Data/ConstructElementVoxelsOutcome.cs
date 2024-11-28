using VoxelService.Data;

namespace VoxelService.Api.Construct.Data;

public class ConstructElementVoxelsOutcome
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Exception? Exception { get; set; }
    public HashSet<Voxel> Voxels { get; set; } = [];

    public static ConstructElementVoxelsOutcome RetrievedVoxels(HashSet<Voxel> voxels) 
        => new() { Success = true, Voxels = voxels };
    
    public static ConstructElementVoxelsOutcome RetrievedCachedVoxels(HashSet<Voxel> voxels) 
        => new() { Success = true, Voxels = voxels };

    public static ConstructElementVoxelsOutcome Failed(Exception exception)
        => new() { Exception = exception, Message = $"Failed to retrieve Element's voxels" };
    
    public static ConstructElementVoxelsOutcome UnknownFailure()
        => new() { Message = "Unknown Failure to retrieve Element's voxels" };
}