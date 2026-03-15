using Microsoft.AspNetCore.Components;

namespace CMSTrain.Client.Layout.Component;

public partial class DisplayFormat
{
    [Parameter] public bool IsDisplayedAsGrid { get; set; } = true;
    
    [Parameter] public EventCallback<bool> IsDisplayedAsGridChanged { get; set; }
    
    private async Task OnValueChanged(bool value)
    {
        await IsDisplayedAsGridChanged.InvokeAsync(value);
    }
}