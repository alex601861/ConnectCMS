using MudBlazor;
using CMSTrain.Client.Service.HTTP;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Interface;
using CMSTrain.Client.Models.Application;
using Microsoft.AspNetCore.Components.Forms;

namespace CMSTrain.Client.Service.Manager;

public class FileManager(ISnackbarService snackbarService, IConfiguration configuration, LocalHttpClient localHttpClient) : IFileManager
{
    private readonly string[] _imageExtensions = [".jpg", ".jpeg", ".png"];
    private readonly string[] _resourceExtensions = [".jpg", ".jpeg", ".png", ".pdf", ".gif", ".svg" ];

    public async Task<string> RenderSvgContent(string path, string fileName)
    {
        try
        {
            var svgFilePath = Path.Combine(path, fileName);
        
            return await localHttpClient.HttpClient.GetStringAsync(svgFilePath);
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }
    
    public bool IsUploadedImageFileValid(IBrowserFile imageFile)
    {
        var isFileSizeValid = IsFileSizeValid(imageFile);

        if (isFileSizeValid)
        {
            var isFileExtensionValid = IsFileExtensionValid(_imageExtensions, imageFile);

            if (isFileExtensionValid)
            {
                return true;
            }
            
            snackbarService.ShowSnackbar(Constants.Message.ImageUploadMessage, Severity.Warning, Variant.Outlined);
        
            return false;
        }

        snackbarService.ShowSnackbar(Constants.Message.FileSizeUploadMessage, Severity.Warning, Variant.Outlined);
        
        return false;
    }

    public bool IsUploadedResourceFileValid(IBrowserFile imageFile)
    {
        var isFileSizeValid = IsFileSizeValid(imageFile);

        if (isFileSizeValid)
        {
            var isFileExtensionValid = IsFileExtensionValid(_resourceExtensions, imageFile);

            if (isFileExtensionValid)
            {
                return true;
            }
            
            snackbarService.ShowSnackbar(Constants.Message.ResourceUploadMessage, Severity.Warning, Variant.Outlined);
        
            return false;
        }

        snackbarService.ShowSnackbar(Constants.Message.FileSizeUploadMessage, Severity.Warning, Variant.Outlined);
        
        return false;
    }

    private static bool IsFileSizeValid(IBrowserFile file)
    {
        var fileSizeInMb = file.Size / (1024 * 1024);

        return fileSizeInMb <= 5;
    }
    
    private static bool IsFileExtensionValid(IEnumerable<string> validExtensions, IBrowserFile file)
    {
        var fileExtension = Path.GetExtension(file.Name).ToLowerInvariant();

        return validExtensions.Contains(fileExtension);
    }

    public string FetchFileUrl(string imageUrl, string path)
    {
        var applicationConfiguration = configuration.GetSection(nameof(Configuration)).Get<Configuration>() 
                                       ?? throw new KeyNotFoundException("The application configuration could not be found, please try again.");

        var baseUrl = applicationConfiguration.ApiUrl;

        var url = $"{baseUrl}/{path}/{imageUrl}";
        
        return url;
    }
}