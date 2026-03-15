using Microsoft.AspNetCore.Components;

namespace CMSTrain.Client.Layout.Component;

public partial class ActivationFilter
{
    [Parameter] public bool? Value { get; set; }

    [Parameter] public EventCallback<bool?> ValueChanged { get; set; }

    private async Task OnValueChanged(bool? value)
    {
        Value = value;
        await ValueChanged.InvokeAsync(value);
    }
}