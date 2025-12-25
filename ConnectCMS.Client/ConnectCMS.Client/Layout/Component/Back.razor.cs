using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace CMSTrain.Client.Layout.Component;

public partial class Back
{
    [Parameter] public string? NavigationUrl { get; set; }
    
    private void GoBack()
    {
        if (string.IsNullOrEmpty(NavigationUrl))
        {
            SnackbarService.ShowSnackbar("Navigation URL has not been set.", Severity.Error, Variant.Outlined);
        }
        else
        {
            NavigationManager.NavigateTo(NavigationUrl);
        }
    }
}