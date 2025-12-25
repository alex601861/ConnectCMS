namespace CMSTrain.Client.Models.Preferences;

public class ClientPreference : IPreference
{
    public string Font { get; set; } = Constants.Constants.FontFamily.Poppins;

    public string PrimaryColor { get; set; } = Constants.Constants.PrimaryColor.Yellow;
}