using CMSTrain.Client.Models.Responses.Menu;
using Microsoft.AspNetCore.Components;

namespace CMSTrain.Client.Layout.Component;

public partial class TreeViewLayout
{
    [Parameter] public RoleMenuResponseDto? Item { get; set; }
}