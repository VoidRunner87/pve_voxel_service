using VoxelService.Api.Construct.Data;

namespace VoxelService.Api.Mesh.Interfaces;

public interface IConstructMeshDownloaderService
{
    Task<MeshDownloadOutcome> DownloadConstructMeshAsync(ulong constructId, byte lod);
}