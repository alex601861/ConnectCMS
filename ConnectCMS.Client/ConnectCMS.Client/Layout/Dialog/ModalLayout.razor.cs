using MudBlazor;
using Microsoft.AspNetCore.Components;

namespace CMSTrain.Client.Layout.Dialog;

public partial class ModalLayout<T> : ComponentBase where T : class
{
    /// <summary>
    /// Parameters for the modal dialog
    /// </summary>
    [Parameter] public string Module { get; set; } = "";

    [Parameter] public string Title { get; set; } = "";

    [Parameter] public string Description { get; set; } = "";

    [Parameter] public Color SubmitColor { get; set; }

    [Parameter] public RenderFragment? ChildContent { get; set; }

    [Parameter] public string CancelLabel { get; set; } = "";

    [Parameter] public string? SubmitLabel { get; set; }

    [Parameter] public string Size { get; set; } = "";

    [Parameter] public string Alignment { get; set; } = "End";

    [Parameter] public EventCallback<bool> OnSave { get; set; }
    
    [Parameter] public bool IsDisabled { get; set; }

    [Parameter] public string? Style { get; set; }

    private T ModelInstance { get; set; } = Activator.CreateInstance<T>();
    
    /// <summary>
    /// Action method to invoke modal cancellation
    /// </summary>
    /// <returns></returns>
    private Task ModalCancel()
    {
        return OnSave.InvokeAsync(true);
    }
    
    /// <summary>
    /// Action method to invoke modal submit
    /// </summary>
    /// <returns></returns>
    private Task ModalSubmit()
    {
        return OnSave.InvokeAsync(false);
    }
}