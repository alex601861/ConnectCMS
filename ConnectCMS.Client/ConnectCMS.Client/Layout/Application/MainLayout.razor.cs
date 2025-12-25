using MudBlazor;
using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Models.Themes;
using CMSTrain.Client.Models.Preferences;
using CMSTrain.Client.Models.Responses.Menu;
using Environment = CMSTrain.Client.Models.Application.Environment;

namespace CMSTrain.Client.Layout.Application;

public partial class MainLayout
{
    public string PageTitle { get; set; } = "Connect CMS";

    public GlobalState? GlobalState { get; private set; }
    
    private bool DrawerOpen { get; set; } = true;
    
    private ClientPreference? ClientPreference { get; set; }

    private MudTheme Theme { get; set; } = new LightTheme();
    
    private List<RoleMenuResponseDto> AssignedMenus { get; set; } = [];
    
    private static bool RightToLeft => false;

    private static bool OpenThemeDrawer { get; set; }

    private bool IsThemeChangeable { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        ClientPreference = await ClientPreferenceManager.GetPreference() as ClientPreference ?? new ClientPreference();

        SetCurrentTheme(ClientPreference);

        var isUserLoggedIn = await AuthenticationService.IsUserLoggedIn();

        if (isUserLoggedIn)
        {
            var userResponse = await ProfileService.GetUserProfile();

            var roleResponse = await MenuService.GetAllAssignedMenus();

            if (userResponse?.Result == null) return;
            
            if (roleResponse?.Result == null) return;

            var userDetails = userResponse.Result;
            
            var roleDetails = roleResponse.Result;
            
            GlobalState = new GlobalState()
            {
                UserId = userDetails.Id,
                Name = userDetails.Name,
                EmailAddress = userDetails.Email,
                RoleName = userDetails.RoleName,
                RoleId = userDetails.RoleId,
                ImageUrl = userDetails.ImageUrl
            };
            
            AssignedMenus = roleDetails;
        }
        else
        {
            var returnUrl = NavigationManager.Uri;

            if (returnUrl.Contains("subordinate-answer-upload-form")) return;
            
            if (!returnUrl.Contains("login") && !returnUrl.Contains("register")) await AuthenticationService.SetUpReturnUrl(returnUrl);
            
            NavigationManager.NavigateTo("/login");
        }
        
        var environmentConfiguration = Configuration.GetSection(nameof(Environment)).Get<Environment>();

        if (environmentConfiguration is { IsProduction: false })
        {
            IsThemeChangeable = true;
        }
    }

    private async Task ThemePreferenceChanged(ClientPreference? themePreference)
    {
        SetCurrentTheme(themePreference ?? new ClientPreference());
        
        await ClientPreferenceManager.SetPreference(themePreference ?? new ClientPreference());
    }
    
    private void SetCurrentTheme(ClientPreference clientPreference)
    {
        var isThemeChanged = clientPreference.PrimaryColor switch
        {
            "Yellow" => false,
            "Orange" => true,
            _ => false
        };
        
        Theme = new LightTheme(isThemeChanged);
        
        Theme.Typography = CustomTypography.CmsTypography(clientPreference.Font);
    }
    
    private async Task LogoutHandler()
    {
        await AuthenticationService.LogOut();

        await AuthenticationService.ClearNavigationUrl();

        NavigationManager.NavigateTo("/login", true);
        
        SnackbarService.ShowSnackbar("You have been logged out successfully.", Severity.Success, Variant.Outlined);
    }
    
    private void DrawerToggle()
    {
        DrawerOpen = !DrawerOpen;
    }
}