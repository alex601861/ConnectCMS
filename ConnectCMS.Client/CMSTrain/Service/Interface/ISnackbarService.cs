using MudBlazor;
using CMSTrain.Client.Service.Dependency;

namespace CMSTrain.Client.Service.Interface;

public interface ISnackbarService : IScopedService
{
    void ShowSnackbar(string message, Severity severity, Variant variant);
}