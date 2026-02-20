using CMSTrain.Application.Common.Service;
using Microsoft.AspNetCore.Http;

namespace CMSTrain.Application.Interfaces.Services;

public interface IFileService : ITransientService
{
    string UploadDocument(IFormFile file, string uploadedFilePath);

    string UploadDocument(string base64Image, string uploadedFilePath);

    // TODO: Delete the existing files for those entities when an image is updated
    void DeleteFile(string uploadedFilePath);

    void DeleteFolder(string folderPath);

    string FileExistPath(string uploadedFilePath);
}