using CMSTrain.Infrastructure.Persistence.Seed;
using Microsoft.Extensions.DependencyInjection;

namespace CMSTrain.Infrastructure.Dependency;

public static class MigrationService
{
    public static async Task AddDataSeedMigrationService(this IServiceCollection services)
    {
        await SeedDefaultDataSets(services);

        await SeedAdmin(services);

        await SeedServiceDataSets(services);
    }

    private static async Task SeedDefaultDataSets(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();

        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();

        await dbInitializer.InitializeCountryDataSets();

        await dbInitializer.InitializeDesignationDataSets();
    }
    
    private static async Task SeedAdmin(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();

        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();

        await dbInitializer.InitializeIdentityData();
    }
    
    private static async Task SeedServiceDataSets(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();

        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();

        await dbInitializer.InitializeMenuData();

        await dbInitializer.InitializeHeadingData();

        await dbInitializer.InitializeStrategicData();

        await dbInitializer.InitializeTraitData();
    }
}