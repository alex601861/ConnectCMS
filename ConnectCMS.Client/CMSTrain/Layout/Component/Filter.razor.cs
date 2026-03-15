using MudBlazor;
using Microsoft.AspNetCore.Components;

namespace CMSTrain.Client.Layout.Component;

public partial class Filter
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
   
    [Parameter] public EventCallback OnFilterApplication { get; set; }

    private MudMenu FilterMenu { get; set; } = new();
    
    private async Task OnApplyFilter(bool isFilterApplied)
    {
        if (isFilterApplied) await OnFilterApplication.InvokeAsync();
        
        await FilterMenu.OpenChanged.InvokeAsync(false);
        
        StateHasChanged();
    }
}