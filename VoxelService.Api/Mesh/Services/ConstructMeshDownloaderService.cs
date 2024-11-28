using VoxelService.Api.Construct.Data;
using VoxelService.Api.Mesh.Interfaces;

namespace VoxelService.Api.Mesh.Services;

public class ConstructMeshDownloaderService(IServiceProvider provider) : IConstructMeshDownloaderService
{
    public async Task<MeshDownloadOutcome> DownloadConstructMeshAsync(ulong constructId, byte lod)
    {
        lod = Math.Clamp(lod, (byte)0, (byte)2);
        
        var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
        var httpClient = httpClientFactory.CreateClient();

        var baseUrl = new Uri(ConfigurationReader.GetMeshDownloadUrl());
        var meshUrl = new Uri(baseUrl, $"/public/voxels/constructs/{constructId}/mesh.glb?async=1&lod={lod}");

        var responseMessage = await httpClient.GetAsync(meshUrl);

        if (!responseMessage.IsSuccessStatusCode)
        {
            return MeshDownloadOutcome.FailedToReadMesh(responseMessage);
        }
        
        var stream = await responseMessage.Content.ReadAsStreamAsync();

        return MeshDownloadOutcome.MeshDownloaded(stream);
    }
}