using Microsoft.AspNetCore.Components;

namespace CMSTrain.Client.Layout.Component;

public partial class Accordion
{
    [Parameter] public RenderFragment CardBody { get; set; }
    
    [Parameter] public RenderFragment CardHeader { get; set; }
    
    [Parameter] public bool ShowCardBody { get; set; } = false;

    private void Toggle()
    {
        ShowCardBody = !ShowCardBody;
    }
}