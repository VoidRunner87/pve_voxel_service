namespace VoxelService.Api.Common;

public static class ServiceProviderHelpers
{
    public static ILogger<T> CreateLogger<T>(this IServiceProvider serviceProvider) =>
        serviceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger<T>();
}