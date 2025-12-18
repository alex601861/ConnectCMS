using Hangfire;
using Hangfire.PostgreSql;
using CMSTrain.Domain.Common;
using CMSTrain.Application.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CMSTrain.Infrastructure.Dependency;

public static class SchedulerService
{
    public static IServiceCollection AddSchedulerService(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseSettings = new DatabaseSettings();

        configuration.GetSection(nameof(DatabaseSettings)).Bind(databaseSettings);

        switch (databaseSettings.DbProvider)
        {
            case Constants.DbProviderKeys.Npgsql:
                services.AddHangfire(x =>
                    x.UsePostgreSqlStorage(databaseSettings.NpgSqlConnectionString));
                break;
            case Constants.DbProviderKeys.SqlServer:
                services.AddHangfire(x =>
                    x.UseSqlServerStorage(databaseSettings.SqlServerConnectionString));
                break;
        }
        
        services.AddHangfireServer();

        return services;
    }
}