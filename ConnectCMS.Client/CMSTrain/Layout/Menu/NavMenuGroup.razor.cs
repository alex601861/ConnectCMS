using CMSTrain.Client.Models.Constants;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Responses.Menu;

namespace CMSTrain.Client.Layout.Menu;

public partial class NavMenuGroup
{
    [Parameter] public RoleMenuResponseDto? Group { get; set; }
}