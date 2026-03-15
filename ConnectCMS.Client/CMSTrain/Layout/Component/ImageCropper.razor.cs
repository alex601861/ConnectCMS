using MudBlazor;
using Blazorise.Cropper;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Requests.File;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace CMSTrain.Client.Layout.Component;

public partial class ImageCropper
{
    [Parameter] 
    public EventCallback<FileUploadResultDto> OnFileUploaded { get; set; }

    private string ImagePreviewUrl { get; set; } = "";
    
    private string DragClass { get; set; } = Constants.Css.DefaultDragClass;
    
    private IBrowserFile? ImageFile { get; set; }
    
    private MudFileUpload<IBrowserFile> FileUpload { get; set; } = new();
    
    private bool IsFileUploadHidden { get; set; }

    private bool IsCropComplete { get; set; }
    
    private async Task OnInputFileChanged(InputFileChangeEventArgs e)
    {
        ClearDragClass();

        var file = e.File;

        var fileSize = new byte[e.File.Size];

        await e.File.OpenReadStream().ReadAsync(fileSize);

        var imageType = e.File.ContentType;

        ImagePreviewUrl = $"data:{imageType};base64,{Convert.ToBase64String(fileSize)}";

        ImageFile = file;

        IsFileUploadHidden = true;

        IsCropComplete = false;
        
        StateHasChanged();
    }

    private void SetDragClass() => DragClass = $"{Constants.Css.DefaultDragClass} mud-border-primary";
    
    private void ClearDragClass() => DragClass = Constants.Css.DefaultDragClass;

    private async Task ClearImage()
    {
        await FileUpload.ClearAsync();

        ImageFile = null;

        IsFileUploadHidden = false;
        
        ImagePreviewUrl = string.Empty;

        IsCropComplete = false;
        
        ClearDragClass();

        StateHasChanged();
    }

    private async Task UploadImage()
    {
        if (!IsCropComplete) return;
        
        var base64Image = await Cropper.CropAsBase64ImageAsync(new CropperCropOptions
        {
            Width = 250,
            Height = 250
        });

        var imageFile = CreateBrowserFileFromBase64(base64Image);

        await OnFileUploaded.InvokeAsync(new FileUploadResultDto()
        {
            File = imageFile,
            Base64File = base64Image
        });

        await ClearImage();
    }

    private Cropper Cropper { get; set; } = new();
    
    private Task OnSelectionChanged(CropperSelectionChangedEventArgs eventArgs)
    {
        IsCropComplete = eventArgs is { Width: > 0, Height: > 0 };
        
        return eventArgs.Width == 0 ? Task.CompletedTask : InvokeAsync(StateHasChanged);
    }

    private IBrowserFile CreateBrowserFileFromBase64(string base64File)
    {
        if (base64File.Contains(','))
        {
            base64File = base64File[(base64File.IndexOf(',') + 1)..];
        }

        var bytes = Convert.FromBase64String(base64File);

        var formFile = new BrowserFile(bytes, ImageFile?.Name ?? "", ImageFile?.ContentType ?? "");

        return formFile;
    }
}