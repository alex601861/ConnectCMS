using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using Microsoft.AspNetCore.Components.Forms;

namespace CMSTrain.Client.Layout.Component;

public partial class ResourceUpload : ComponentBase
{
    [Parameter] public string UploadType { get; set; } = "";

    [Parameter] public string AcceptedExtensions { get; set; } = "";
    
    [Parameter] public string FileUploadedMessage { get; set; } = "";
    
    [Parameter] public EventCallback<IBrowserFile?> OnFileUploaded { get; set; }
    
    private bool IsUploadedFileValid { get; set; }

    private string FileName { get; set; } = "";
    
    private async Task OnResourceUpload(IBrowserFile? file)
    {
        try
        {
            if (file != null)
            {
                var isFileValid = UploadType switch
                {
                    Constants.Uploads.Image => FileManager.IsUploadedImageFileValid(file),
                    Constants.Uploads.Resource => FileManager.IsUploadedResourceFileValid(file),
                    _ => false
                };

                if (isFileValid)
                {
                    IsUploadedFileValid = true;
                    
                    FileName = file.Name;
                    
                    var buffer = new byte[file.Size];
                    
                    await using var stream = file.OpenReadStream(5 * 1024 * 1024);
                    
                    await stream.ReadExactlyAsync(buffer, 0, (int)file.Size);
                    
                    await OnFileUploaded.InvokeAsync(file);
                    
                    StateHasChanged();

                    return;
                }

                IsUploadedFileValid = false;

                StateHasChanged();
            }
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Warning, Variant.Outlined);
        }
    }
    
    private async Task OnFileRemoval()
    {
        FileName = "";
        
        IsUploadedFileValid = false;
        
        await OnFileUploaded.InvokeAsync(null);
        
        StateHasChanged();
    }
}