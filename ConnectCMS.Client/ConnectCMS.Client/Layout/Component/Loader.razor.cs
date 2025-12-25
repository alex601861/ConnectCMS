using Microsoft.AspNetCore.Components;
using Environment = CMSTrain.Client.Models.Application.Environment;

namespace CMSTrain.Client.Layout.Component;

// TODO: Invoke the following component for details fetch as well.
public partial class Loader
{
    [Parameter] public string Type { get; set; } = "GIF";

    protected override void OnInitialized()
    {
        var environmentConfiguration = Configuration.GetSection(nameof(Environment)).Get<Environment>();

        if (environmentConfiguration is { IsProduction: false })
        {
            Type = "Circular Progress";
        }
    }
}