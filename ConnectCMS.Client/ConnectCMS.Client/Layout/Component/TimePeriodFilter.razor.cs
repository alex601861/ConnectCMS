using Microsoft.AspNetCore.Components;

namespace CMSTrain.Client.Layout.Component;

public partial class TimePeriodFilter
{
    [Parameter] public int Value { get; set; }
    
    [Parameter] public EventCallback<int> ValueChanged { get; set; }

    private async Task OnValueChanged(int value)
    {
        Value = value;
        await ValueChanged.InvokeAsync(value);
    }
}