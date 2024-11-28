using Backend;
using Orleans;

namespace VoxelService.Api.Common.Extensions;

public static class ServiceProviderExtensions
{
    public static ILogger<T> CreateLogger<T>(this IServiceProvider serviceProvider) =>
        serviceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger<T>();

    public static IClusterClient GetOrleans(this IServiceProvider serviceProvider)
        => serviceProvider.GetRequiredService<IClusterClient>();
    
    public static IGameplayBank GetGameplayBank(this IServiceProvider serviceProvider)
        => serviceProvider.GetRequiredService<IGameplayBank>();
}