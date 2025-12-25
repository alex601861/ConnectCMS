using MudBlazor;

namespace CMSTrain.Client.Models.Themes;

public class LightTheme : MudTheme
{
    public LightTheme(bool isPrimaryColorChanged = false)
    {
        PaletteLight = new PaletteLight()
        {
            Primary = !isPrimaryColorChanged ? "#F1973C" : "#F67F5B",
            PrimaryLighten = !isPrimaryColorChanged ? "#FFE8C8" : "#FDE4DC",
            Secondary = "#005399",
            Success = "#00cc29",
            Error = "#ff0000",
            Tertiary = "#ff00001a",
            TertiaryContrastText = "#ff0000",
            TertiaryDarken = "#fff",
            Info = "#0bc5ea",
            Background = "#f8f8fa",
            AppbarBackground = "#fff",
            AppbarText = "#141414",
            DrawerBackground = "#fff",
            DrawerText = "rgba(0,0,0, 0.7)",
            TableLines = "#ebebeb",
            OverlayDark = "hsl(0deg 0% 0% / 75%)",
            Divider = "#ebebeb",
            TextPrimary = "#141414",
            TextSecondary = "#5c5c5c",
            GrayLight = "#858585",
            White = "#fff"
        };
        Shadows = new Shadow();
        Typography = CustomTypography.CmsTypography();
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "5px"
        };
        ZIndex = new ZIndex
        {
            Drawer = 1300
        };
    }
}