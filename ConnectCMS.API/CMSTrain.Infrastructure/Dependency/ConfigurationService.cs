using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace CMSTrain.Infrastructure.Dependency;

public static class ConfigurationService
{
    public static WebApplicationBuilder AddConfigurations(this WebApplicationBuilder builder)
    {
        var environment = builder.Environment;
        
        var configuration = builder.Configuration;
        
        Console.WriteLine($"Environment: {environment.EnvironmentName}.");

        configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile("menu.json", optional: false, reloadOnChange: false)
            .AddJsonFile($"menu.{environment.EnvironmentName}.json", optional: true, reloadOnChange: false)
            .AddJsonFile("client.json", optional: false, reloadOnChange: false)
            .AddJsonFile($"client.{environment.EnvironmentName}.json", optional: true, reloadOnChange: false)
            .AddJsonFile("questionnaire.json", optional: false, reloadOnChange: false)
            .AddJsonFile($"questionnaire.{environment.EnvironmentName}.json", optional: true, reloadOnChange: false);

        return builder;
    }
}