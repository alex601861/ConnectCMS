using MudBlazor;
using CMSTrain.Client.Models.Preferences;
using CMSTrain.Client.Models.Themes;

namespace CMSTrain.Client.Layout.Application;

public partial class EmptyLayout
{
    private MudTheme MudTheme { get; set; } = new LightTheme();

    private ClientPreference? ClientPreference { get; set; }

    protected override async Task OnInitializedAsync()
    {
        ClientPreference = await ClientPreferenceManager.GetPreference() as ClientPreference ?? new ClientPreference();

        SetCurrentTheme(ClientPreference);
    }
    
    private void SetCurrentTheme(ClientPreference clientPreference)
    {
        var isThemeChanged = clientPreference.PrimaryColor switch
        {
            "Yellow" => false,
            "Orange" => true,
            _ => false
        };
        
        MudTheme = new LightTheme(isThemeChanged);
        
        MudTheme.Typography = CustomTypography.CmsTypography(clientPreference.Font);
    }
}