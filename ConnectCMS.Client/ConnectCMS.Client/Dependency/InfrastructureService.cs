using Blazorise;
using MudBlazor;
using Syncfusion.Blazor;
using MudBlazor.Services;
using Blazorise.Bootstrap5;
using Blazored.LocalStorage;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Builder;
using CMSTrain.Client.Service.HTTP;
using CMSTrain.Client.Service.Manager;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.StaticFiles;
using CMSTrain.Client.Models.Application;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Environment = CMSTrain.Client.Models.Application.Environment;

namespace CMSTrain.Client.Dependency;

public static class InfrastructureService
{
    public static void AddInfrastructureService(this IServiceCollection services, WebAssemblyHostBuilder builder)
    {
        var configuration = builder.Configuration;
        
        var environment = builder.HostEnvironment;

        Console.WriteLine($"Environment: {builder.HostEnvironment.Environment}.");
        Console.WriteLine($"Base Address: {builder.HostEnvironment.BaseAddress}.");

        configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment.Environment}.json", optional: true, reloadOnChange: true);
            
        var applicationConfiguration = configuration.GetSection(nameof(Configuration)).Get<Configuration>()
            ?? throw new KeyNotFoundException("The application configuration could not be found, please try again.");

        var syncfusionConfiguration = configuration.GetSection(nameof(SyncfusionSettings)).Get<SyncfusionSettings>() 
                                      ?? throw new KeyNotFoundException("The syncfusion configuration could not be found, please try again.");
        
        var environmentConfiguration = configuration.GetSection(nameof(Environment)).Get<Environment>() 
                                      ?? throw new KeyNotFoundException("The environment configuration could not be found, please try again.");

        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(syncfusionConfiguration.LicenseKey);

        services.AddDependencyServices();

        services.AddSingleton<JwtSecurityTokenHandler>();
        
        services.AddScoped<IdentityAuthenticationStateManager>();
        
        services.AddScoped<AuthenticationStateProvider>(p =>
            p.GetRequiredService<IdentityAuthenticationStateManager>());
        
        services.AddScoped(_ => new ApiHttpClient(new HttpClient(), applicationConfiguration.ApiUrl));
        
        services.AddScoped(_ => new LocalHttpClient(new HttpClient(), builder.HostEnvironment.BaseAddress));

        services.AddLocalization();

        services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;

            config.SnackbarConfiguration.NewestOnTop = false;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.PreventDuplicates = false;
            config.SnackbarConfiguration.VisibleStateDuration = 10000;
            config.SnackbarConfiguration.HideTransitionDuration = 500;
            config.SnackbarConfiguration.ShowTransitionDuration = 500;
        });

        services
            .AddBlazorise(options =>
            {
                options.Immediate = true;
            })
            .AddBootstrap5Providers()
            .AddFontAwesomeIcons();

        services.AddSyncfusionBlazor();

        services.AddBlazoredLocalStorage();

        services.AddAuthorizationCore();
        
        services.Configure<StaticFileOptions>(options =>
        {
            options.ServeUnknownFileTypes = true;
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings.Add(".js", "application/javascript");
            options.ContentTypeProvider = provider;
        });
    }
}