using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Responses.Menu;

namespace CMSTrain.Client.Layout.Menu;

public partial class NavMenu
{
    public string Search { get; set; } = "";

    [Parameter] public List<RoleMenuResponseDto>? MenuItems { get; set; }
}