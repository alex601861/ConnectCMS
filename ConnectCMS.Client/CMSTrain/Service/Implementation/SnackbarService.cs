using MudBlazor;
using CMSTrain.Client.Service.Interface;

namespace CMSTrain.Client.Service.Implementation;

public class SnackbarService(ISnackbar snackbar) : ISnackbarService
{
    public void ShowSnackbar(string message, Severity severity, Variant variant)
    {
        snackbar.Add(message, severity, c => c.SnackbarVariant = variant);
    }
}