using Microsoft.AspNetCore.Components;

namespace CMSTrain.Client.Layout.Component;

public partial class Message
{
    [Parameter] public string Icon { get; set; }
    
    [Parameter] public string Title { get; set; } = "";

    [Parameter] public string Description { get; set; } = "";
    
    [Parameter] public RenderFragment? ChildContent { get; set; }
}