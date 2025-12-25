using MudBlazor;
using CMSTrain.Client.Service.Manager;
using Microsoft.AspNetCore.Components;
using Constants = CMSTrain.Client.Models.Constants.Constants;

namespace CMSTrain.Client.Layout.Authorization;

public partial class RedirectToLogin
{
    [Inject] private ILocalStorageManager LocalStorageManager { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        var currentUri = NavigationManager.Uri;
        
        if (currentUri.Contains("login", StringComparison.OrdinalIgnoreCase)) return;
        
        await LocalStorageManager.SetItemAsync(Constants.LocalStorage.Navigation, currentUri);
        
        SnackbarService.ShowSnackbar("You need to login to access this page :).", Severity.Warning, Variant.Outlined);
        
        NavigationManager.NavigateTo("login");
    }
}