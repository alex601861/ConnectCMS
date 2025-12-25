using CMSTrain.Client.Models.Constants;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CMSTrain.Client.Layout.Component;

// TODO: Invoke of the following component through out the application
public partial class Avatar
{
    [Parameter] public string Name { get; set; } = "A";

    [Parameter] public GenderType? Gender { get; set; }
    
    [Parameter] public string? ImageUrl { get; set; } = "";

    [Parameter] public string? FilePath { get; set; } = "";

    [Parameter] public decimal Height { get; set; } = 77;
    
    [Parameter] public decimal Width { get; set; } = 77;

    [Parameter] public bool IsMarginRequired { get; set; } = true;

    private string AvatarStyle { get; set; } = "";

    private string GenderIcon { get; set; } = "";

    private Color GenderColor { get; set; } = new();
    
    protected override void OnInitialized()
    {
        AvatarStyle = $"width: {Width}px; height: {Height}px;";

        if (Gender == null) return;
        
        if (Gender == GenderType.Male)
        {
            GenderIcon = Icons.Material.Filled.Male;
            GenderColor = Color.Info;
        }
        else if (Gender == GenderType.Female)
        {
            GenderIcon = Icons.Material.Filled.Female;
            GenderColor = Color.Tertiary;
        }
        else if (Gender == GenderType.Other)
        {
            GenderIcon = Icons.Material.Filled.Transgender;
            GenderColor = Color.Primary;
        }
    }
    
    private string GetFilePath()
    {
        if (!string.IsNullOrEmpty(ImageUrl) && !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(FilePath))
            return FileManager.FetchFileUrl(ImageUrl, FilePath);

        return string.Empty;
    }
}