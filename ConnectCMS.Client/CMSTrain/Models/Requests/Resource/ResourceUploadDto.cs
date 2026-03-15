using Microsoft.AspNetCore.Components.Forms;

namespace CMSTrain.Client.Models.Requests.Resource;

public class ResourceUploadDto
{
    public string Title { get; set; }

    public bool IsLink { get; set; }

    public string Description { get; set; }

    public string? Link { get; set; }

    public IBrowserFile? ResourceFile { get; set; }
}
