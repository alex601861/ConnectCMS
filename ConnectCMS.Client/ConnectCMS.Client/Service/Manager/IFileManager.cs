using CMSTrain.Client.Service.Dependency;
using Microsoft.AspNetCore.Components.Forms;

namespace CMSTrain.Client.Service.Manager;

public interface IFileManager : ITransientService
{
    Task<string> RenderSvgContent(string path, string fileName);
    
    bool IsUploadedImageFileValid(IBrowserFile imageFile);

    bool IsUploadedResourceFileValid(IBrowserFile imageFile);

    string FetchFileUrl(string imageUrl, string path);
}