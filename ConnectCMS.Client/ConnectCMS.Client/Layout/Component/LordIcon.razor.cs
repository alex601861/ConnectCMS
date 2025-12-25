using Microsoft.AspNetCore.Components;

namespace CMSTrain.Client.Layout.Component;

public partial class LordIcon
{
    [Parameter] public string Icon { get; set; } = "";
    
    [Parameter] public string Trigger { get; set; } = "loop";

    [Parameter] public string? State { get; set; } = "";
    
    [Parameter] public decimal Width { get; set; }
    
    [Parameter] public decimal Height { get; set; }

    [Parameter] public string PrimaryColor { get; set; } = "#005399";

    [Parameter] public string SecondaryColor { get; set; } = "#ff0000";
    
    private string GetWidthInPixel() => $"{Width}px";
    
    private string GetHeightInPixel() => $"{Height}px";
}