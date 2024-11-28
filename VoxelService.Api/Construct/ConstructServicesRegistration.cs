using VoxelService.Api.Construct.Interfaces;
using VoxelService.Api.Construct.Services;

namespace VoxelService.Api.Construct;

public static class ConstructServicesRegistration
{
    public static void RegisterConstructServices(this IServiceCollection services)
    {
        services.AddSingleton<IConstructElementVoxelReaderService>(
            provider => new CachedConstructElementVoxelReaderService(
                new ConstructElementVoxelReaderService(provider)
            )
        );
    }
}