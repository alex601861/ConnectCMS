using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.Menu;

namespace CMSTrain.Client.Layout.Menu;

public partial class NavMenuItem
{
    [Parameter] public RoleMenuResponseDto? Item { get; set; }

    private string Icon { get; set; } = Constants.Menu.DefaultIcon;

    protected override async Task OnInitializedAsync()
    {
        if (Item == null) return;

        var svgContent = await FileManager.RenderSvgContent(Constants.Menu.Path, $"{Item.Title.ToLower()}.svg");

        Icon = string.IsNullOrEmpty(svgContent) ? Constants.Menu.DefaultIcon : svgContent;
    }
}