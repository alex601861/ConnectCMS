using CMSTrain.Client.Models.Preferences;
using Microsoft.AspNetCore.Components;

namespace CMSTrain.Client.Layout.Theme;

public partial class ThemeDrawer
{
    [Parameter]
    public bool ThemeDrawerOpen { get; set; }

    [Parameter]
    public EventCallback<bool> ThemeDrawerOpenChanged { get; set; }

    [Parameter]
    [EditorRequired]
    public ClientPreference ThemePreference { get; set; } = default!;

    [EditorRequired]
    [Parameter]
    public EventCallback<ClientPreference?> ThemePreferenceChanged { get; set; }

    private async Task UpdateTheme()
    {
        await ThemePreferenceChanged.InvokeAsync(ThemePreference);
    }
}