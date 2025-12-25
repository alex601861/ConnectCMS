using CMSTrain.Client;
using CMSTrain.Client.Dependency;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var rootComponents = builder.RootComponents;

rootComponents.Add<App>("#app");

rootComponents.Add<HeadOutlet>("head::after");

var services = builder.Services;

services.AddInfrastructureService(builder);

await builder.Build().RunAsync();