using VoxelService.Api.Mesh.Interfaces;
using VoxelService.Api.Mesh.Services;

namespace VoxelService.Api.Mesh;

public static class MeshServicesRegistration
{
    public static void RegisterMeshServices(this IServiceCollection services)
    {
        services.AddSingleton<IConstructMeshDownloaderService, ConstructMeshDownloaderService>();
    }
}