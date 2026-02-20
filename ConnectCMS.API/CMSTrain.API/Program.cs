using Serilog;
using Hangfire;
using CMSTrain.Middleware;
using CMSTrain.Domain.Common;
using CMSTrain.Configurations;
using CMSTrain.Identity.Dependency;
using System.Text.Json.Serialization;
using System.IdentityModel.Tokens.Jwt;
using Swashbuckle.AspNetCore.SwaggerUI;
using CMSTrain.Infrastructure.Dependency;
using CMSTrain.Infrastructure.Scheduler;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

var configuration = builder.Configuration;

builder.AddConfigurations(); 

services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

services.AddEndpointsApiExplorer(); 

services.AddDependencyServices(); 

services.AddIdentityServices(configuration); 

services.AddInfrastructureService(configuration); 

services.AddSchedulerService(configuration); 

await services.AddDataSeedMigrationService(); 

services.AddCustomSwaggerGen();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration).CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

if (app.Environment.IsDevelopment() || app.Environment.IsStaging() || app.Environment.IsProduction())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.DocExpansion(DocExpansion.None);
        options.DefaultModelsExpandDepth(-1);
        options.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
    });
}

app.UseHangfireDashboard("/hangfire/dashboard", new DashboardOptions
{               
    DashboardTitle = "Connect CMS - Hangfire Dashboard",
    Authorization =
    [
        new HangfireAuthenticationFilter
        {
            User = Constants.SuperAdmin.Hangfire.Username,
            Password = Constants.SuperAdmin.Hangfire.DecryptedPassword
        }
    ]
});

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, OPTIONS");
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type");
    }
});

app.UseHttpsRedirection();

app.UseCors(Constants.Cors.MyAllowSpecificOrigins);

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();