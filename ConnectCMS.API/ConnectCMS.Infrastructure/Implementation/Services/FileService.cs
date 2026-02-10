using SkiaSharp;
using CMSTrain.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using CMSTrain.Application.Interfaces.Services;

namespace CMSTrain.Infrastructure.Implementation.Services;

public class FileService(IWebHostEnvironment webHostEnvironment) : IFileService
{
    public string UploadDocument(IFormFile file, string uploadedFilePath)
    {
        if (!Directory.Exists(Path.Combine(webHostEnvironment.WebRootPath, uploadedFilePath)))
        {
            Directory.CreateDirectory(Path.Combine(webHostEnvironment.WebRootPath, uploadedFilePath));
        }

        var uploadedDocumentPath = Path.Combine(webHostEnvironment.WebRootPath, uploadedFilePath);

        var fileName = UploadFile(uploadedDocumentPath, file);

        return fileName;
    }

    public string UploadDocument(string base64Image, string uploadedFilePath)
    {
        if (!Directory.Exists(Path.Combine(webHostEnvironment.WebRootPath, uploadedFilePath)))
        {
            Directory.CreateDirectory(Path.Combine(webHostEnvironment.WebRootPath, uploadedFilePath));
        }

        var base64Data = base64Image.Split(',').Last();

        var imageBytes = Convert.FromBase64String(base64Data);

        const string extension = ".jpg";

        var fileName = extension.SetUniqueFileName();

        var uploadedDocumentPath = Path.Combine(webHostEnvironment.WebRootPath, uploadedFilePath, fileName);

        using var ms = new MemoryStream(imageBytes);
        
        var originalImage = SKBitmap.Decode(ms);

        var newBitmap = new SKBitmap(originalImage.Width, originalImage.Height);
            
        using (var canvas = new SKCanvas(newBitmap))
        {
            canvas.Clear(SKColors.White);
            canvas.DrawBitmap(originalImage, 0, 0);
        }

        using (var outputStream = File.OpenWrite(uploadedDocumentPath))
        {
            newBitmap.Encode(outputStream, SKEncodedImageFormat.Jpeg, 100);
        }

        return fileName;
    }
    
    private static string UploadFile(string uploadedFilePath, IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName);

        var fileName = extension.SetUniqueFileName();

        using var stream = new FileStream(Path.Combine(uploadedFilePath, fileName), FileMode.Create);

        file.CopyTo(stream);

        return fileName;
    }

    public void DeleteFile(string uploadedFilePath)
    {
        var fullPath = Path.Combine(webHostEnvironment.WebRootPath, uploadedFilePath);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    public void DeleteFolder(string folderPath)
    {
        var fullPath = Path.Combine(webHostEnvironment.WebRootPath, folderPath);

        if (Directory.Exists(fullPath))
        {
            Directory.Delete(fullPath, recursive: true);
        }
    }
    
    public string FileExistPath(string uploadedFilePath)
    {
        var fullPath = Path.Combine(webHostEnvironment.WebRootPath, uploadedFilePath);

        return File.Exists(fullPath) ? fullPath : "";
    }
}