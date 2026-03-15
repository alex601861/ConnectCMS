using Microsoft.AspNetCore.Components.Forms;

namespace CMSTrain.Client.Models.Requests.File;

public class FileUploadResultDto
{
    public IBrowserFile File { get; set; }
    
    public string? Base64File { get; set; }
}