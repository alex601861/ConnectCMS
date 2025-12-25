using MudBlazor;

namespace CMSTrain.Client.Models.Components;

public class Button : MudButton
{
    protected override void OnParametersSet()
    {
        Class = $"{Class} btn btn--sm";
        DropShadow = false;
        base.OnParametersSet();
    }
}