using Backend;
using NQ.Router;
using NQutils;
using NQutils.Sql;
using Services;

namespace VoxelService.Api.DU;

public static class DualUniverseDependenciesRegistration
{
    public static void RegisterDualUniverseDependencies(this IServiceCollection services)
    {
        services.AddOrleansClient("VoxelService");
        services.AddInitializableSingleton<IGameplayBank, GameplayBank>();
        services.AddInitializableSingleton<ISql, Sql>();
        services.AddInitializableSingleton<IElementBoundingBox, ElementBoundingBox>();
    }
}