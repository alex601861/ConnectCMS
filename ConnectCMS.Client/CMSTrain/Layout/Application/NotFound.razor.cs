using MudBlazor;
using CMSTrain.Client.Layout.Theme;
using CMSTrain.Client.Models.Themes;

namespace CMSTrain.Client.Layout.Application;

public partial class NotFound
{
    private MudTheme _currentTheme = new LightTheme();

    protected override void OnInitialized()
    {
        _currentTheme = new LightTheme();
    }
}