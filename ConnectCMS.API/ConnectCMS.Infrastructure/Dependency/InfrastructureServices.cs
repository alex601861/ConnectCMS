using CMSTrain.Helper;
using CMSTrain.Domain.Common;
using CMSTrain.Application.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using CMSTrain.Infrastructure.Persistence;
using CMSTrain.Application.Interfaces.Data;
using Microsoft.Extensions.DependencyInjection;

namespace CMSTrain.Infrastructure.Dependency;

public static class InfrastructureServices
{
    public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseSettings = new DatabaseSettings();

        configuration.GetSection(nameof(DatabaseSettings)).Bind(databaseSettings);

        var connectionString = databaseSettings.DbProvider == Constants.DbProviderKeys.Npgsql
            ? databaseSettings.NpgSqlConnectionString
            : databaseSettings.SqlServerConnectionString;

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseDatabase(databaseSettings.DbProvider, connectionString!);
        });

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetService<ApplicationDbContext>()!);

        services.AddHttpClient();

        services.AddDistributedMemoryCache();

        EnsureDatabaseMigrated(services);
        
        services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));

        services.Configure<MenuSettings>(configuration.GetSection(nameof(MenuSettings)));

        services.Configure<ClientSettings>(configuration.GetSection(nameof(ClientSettings)));

        services.Configure<HeadingSettings>(configuration.GetSection(nameof(HeadingSettings)));

        services.Configure<StrategicSettings>(configuration.GetSection(nameof(StrategicSettings)));

        services.Configure<TraitSettings>(configuration.GetSection(nameof(TraitSettings)));
        
        services.EnableCors(configuration);

        return services;
    }

    private static void EnsureDatabaseMigrated(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Database.Migrate();
    }
    
    private static void EnableCors(this IServiceCollection services, IConfiguration configuration)
    {
        var clientSettings = new ClientSettings();

        configuration.GetSection(nameof(ClientSettings)).Bind(clientSettings);

        var baseUrls = clientSettings.BaseUrl.Split(";");

        foreach (var baseUrl in baseUrls)
        {
            Console.WriteLine($"CORS Allowed Environment: {baseUrl}.");
        }

        services.AddCors(options =>
        {
            options.AddPolicy(name: Constants.Cors.MyAllowSpecificOrigins,
                builder =>
                {
                    builder.WithOrigins(baseUrls)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });
    }
}
