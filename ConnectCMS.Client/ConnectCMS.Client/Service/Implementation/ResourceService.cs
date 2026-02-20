using Microsoft.JSInterop;
using System.Net.Http.Headers;
using System.Text.Json;
using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Interface;
using CMSTrain.Client.Models.Requests.Resource;
using CMSTrain.Client.Models.Responses.Resource;

namespace CMSTrain.Client.Service.Implementation;

public class ResourceService(IBaseService baseService, IJSRuntime jsRuntime) : IResourceService
{
    public async Task<ResponseDto<GetResourceDetailsDto?>?> GetResourceById(Guid resourceId)
    {
        var pathParameter = new List<string>()
        {
            resourceId.ToString()
        };
        
        var response = await baseService.GetAsync<GetResourceDetailsDto?>(ApiEndpoints.Resource.GetResourceById, pathParameter);

        return response;
    }

    public async Task<ResponseDto<GetResourceModuleDetailsDto?>?> GetTrainingResourceById(Guid trainingResourceId)
    {
        var pathParameter = new List<string>()
        {
            trainingResourceId.ToString()
        };
        
        var response = await baseService.GetAsync<GetResourceModuleDetailsDto?>(ApiEndpoints.Resource.GetTrainingResourceById, pathParameter);

        return response;
    }

    public async Task<ResponseDto<GetResourceModuleDetailsDto?>?> GetClassResourceById(Guid classResourceId)
    {
        var pathParameter = new List<string>()
        {
            classResourceId.ToString()
        };
        
        var response = await baseService.GetAsync<GetResourceModuleDetailsDto?>(ApiEndpoints.Resource.GetClassResourceById, pathParameter);

        return response;
    }

    public async Task<CollectionDto<GetResourceDetailsDto>?> GetAllResources(int pageNumber, int pageSize, string? search = null)
    {
        var queryParameter = new Dictionary<string, string?>
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search }
        };
        
        var response = await baseService.GetPagedAsync<GetResourceDetailsDto>(endpoint: ApiEndpoints.Resource.GetAllResources, parameters: queryParameter);

        return response;
    }
    
    public async Task<ResponseDto<List<GetResourceDetailsDto>?>?> GetAllResources(string? search = null)
    {
        var queryParameter = new Dictionary<string, string?>
        {
            { "search", search }
        };
        
        var response = await baseService.GetAsync<List<GetResourceDetailsDto>?>(ApiEndpoints.Resource.GetAllResourcesList, parameters: queryParameter);

        return response;
    }

    public async Task<CollectionDto<GetResourceModuleDetailsDto>?> GetAllResourcesForTraining(Guid trainingId, int pageNumber, int pageSize, string? search = null, bool? isActive = null)
    {
        var pathParameter = new List<string>()
        {
            trainingId.ToString()
        };
        
        var queryParameter = new Dictionary<string, string?>
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search },
            { "isActive", isActive?.ToString() }
        };
        
        var response = await baseService.GetPagedAsync<GetResourceModuleDetailsDto>(ApiEndpoints.Resource.GetAllResourcesForTraining, pathParameter, queryParameter);

        return response;
    }
    
    public async Task<ResponseDto<List<GetResourceModuleDetailsDto>?>?> GetAllResourcesForTraining(Guid trainingId, string? search = null, bool? isActive = null)
    {
        var pathParameter = new List<string>()
        {
            trainingId.ToString()
        };
        
        var queryParameter = new Dictionary<string, string?>
        {
            { "search", search },
            { "isActive", isActive?.ToString() }
        };
        
        var response = await baseService.GetAsync<List<GetResourceModuleDetailsDto>?>(ApiEndpoints.Resource.GetAllResourcesForTrainingList, pathParameter, queryParameter);

        return response;
    }
    
    public async Task<CollectionDto<GetResourceModuleDetailsDto>?> GetAllResourcesForClass(Guid classId, int pageNumber, int pageSize, string? search = null, bool? isActive = null)
    {
        var pathParameter = new List<string>()
        {
            classId.ToString()
        };
        
        var queryParameter = new Dictionary<string, string?>
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search },
            { "isActive", isActive?.ToString() }
        };
        
        var response = await baseService.GetPagedAsync<GetResourceModuleDetailsDto>(ApiEndpoints.Resource.GetAllResourcesForClass, pathParameter, queryParameter);

        return response;
    }
    
    public async Task<ResponseDto<List<GetResourceModuleDetailsDto>?>?> GetAllResourcesForClass(Guid classId, string? search = null, bool? isActive = null)
    {
        var pathParameter = new List<string>()
        {
            classId.ToString()
        };
        
        var queryParameter = new Dictionary<string, string?>
        {
            { "search", search },
            { "isActive", isActive?.ToString() }
        };
        
        var response = await baseService.GetAsync<List<GetResourceModuleDetailsDto>?>(ApiEndpoints.Resource.GetAllResourcesForClassList, pathParameter, queryParameter);

        return response;
    }

    public async Task<ResponseDto<bool?>?> ActivateDeactivateResourceForTraining(Guid trainingResourceId)
    {
        var pathParameter = new List<string>()
        {
            trainingResourceId.ToString()
        };
        
        var response = await baseService.DeleteAsync<bool?>(ApiEndpoints.Resource.ActivateDeactivateResourceForTraining, Constants.UpdateType.Patch, pathParameter);

        return response;
    }

    public async Task<ResponseDto<bool?>?> ActivateDeactivateResourceForClass(Guid classResourceId)
    {
        var pathParameter = new List<string>()
        {
            classResourceId.ToString()
        };
        
        var response = await baseService.DeleteAsync<bool?>(ApiEndpoints.Resource.ActivateDeactivateResourceForClass, Constants.UpdateType.Patch, pathParameter);

        return response;
    }
    
    public async Task<ResponseDto<bool?>?> UploadResources(ResourceUploadDto resource)
    {
        var formData = new MultipartFormDataContent();

        formData.Add(new StringContent(resource.Title), "resource.Title");
        formData.Add(new StringContent(resource.Description), "resource.Description");
        formData.Add(new StringContent(resource.IsLink.ToString()), "resource.IsLink");

        if (resource.IsLink)
        {
            formData.Add(new StringContent(resource.Link ?? ""), "resource.Link");
        }
        else
        {
            var fileContent = new StreamContent(resource.ResourceFile!.OpenReadStream(long.MaxValue));
            
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(resource.ResourceFile.ContentType);
            
            formData.Add(content: fileContent, name: "resource.ResourceFile", fileName: resource.ResourceFile.Name);
        }

        var response = await baseService.UploadAsync<bool?>(ApiEndpoints.Resource.UploadResources, Constants.UploadType.Post, formData);

        return response;
    }

    public async Task<ResponseDto<bool?>?> UploadResourcesPost(ResourcePostDto resource)
    {
        var jsonRequest = JsonSerializer.Serialize(resource);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.PostAsync<bool?>(ApiEndpoints.Resource.UploadResourcesPost, content);
        
        return response;
    }

    public async Task<ResponseDto<bool?>?> UpdateResource(UpdateResourceDto resource)
    {
        var formData = new MultipartFormDataContent();
        
        formData.Add(new StringContent(resource.Id.ToString()), "resource.Id");
        formData.Add(new StringContent(resource.Title), "resource.Title");
        formData.Add(new StringContent(resource.Description), "resource.Description");
        formData.Add(new StringContent(resource.IsLink.ToString()), "resource.IsLink");

        if (resource.IsLink)
        {
            formData.Add(new StringContent(resource.Link ?? ""), "resource.Link");
        }
        else
        {
            if (resource.ResourceFile != null)
            {
                var fileContent = new StreamContent(resource.ResourceFile.OpenReadStream(long.MaxValue));
                
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(resource.ResourceFile.ContentType);
                
                formData.Add(content: fileContent, name: "resource.ResourceFile", fileName: resource.ResourceFile.Name);
            }
        }

        var response = await baseService.UploadAsync<bool?>(ApiEndpoints.Resource.UpdateResource, Constants.UploadType.Put, formData);
        
        return response;
    }

    public async Task<ResponseDto<bool?>?> UpdateResource(ResourcePostUpdateDto resourcePostUpdate)
    {
        var jsonRequest = JsonSerializer.Serialize(resourcePostUpdate);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.UpdateAsync<bool?>(ApiEndpoints.Resource.UpdateResourcePost, Constants.UploadType.Put, content);
        
        return response;
    }

    public async Task<ResponseDto<bool?>?> UploadResourceModule(ResourceModuleUploadDto resource)
    {
        var formData = new MultipartFormDataContent();

        formData.Add(new StringContent(resource.ModuleId.ToString()), "resource.ModuleId");
        formData.Add(new StringContent(resource.IsMaterialForTraining.ToString()), "resource.IsMaterialForTraining");

        var resourceMaterial = resource.Resource;
        
        formData.Add(new StringContent(resourceMaterial.Title), "resource.Resource.Title");
        formData.Add(new StringContent(resourceMaterial.Description), "resource.Resource.Description");
        formData.Add(new StringContent(resourceMaterial.IsLink.ToString()), "resource.Resource.IsLink");

        if (resourceMaterial.IsLink)
        {
            formData.Add(new StringContent(resourceMaterial.Link ?? ""), "resource.Resource.Link");
        }
        else
        {
            var fileContent = new StreamContent(resourceMaterial.ResourceFile!.OpenReadStream(long.MaxValue));
            
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(resourceMaterial.ResourceFile.ContentType);
            
            formData.Add(content: fileContent, name: "resource.Resource.ResourceFile", fileName: resourceMaterial.ResourceFile.Name);
        }

        var response = await baseService.UploadAsync<bool?>(ApiEndpoints.Resource.UploadResourceModule, Constants.UploadType.Post, formData);

        return response;
    }
    
    public async Task<ResponseDto<bool?>?> UploadResourcesForTraining(TrainingResourceUploadDto resource)
    {
        var jsonRequest = JsonSerializer.Serialize(resource);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.PostAsync<bool?>(ApiEndpoints.Resource.UploadResourcesForTraining, content);
        
        return response;
    }
    
    public async Task<ResponseDto<bool?>?> UploadResourcesForClass(ClassResourceUploadDto resource)
    {
        var jsonRequest = JsonSerializer.Serialize(resource);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.PostAsync<bool?>(ApiEndpoints.Resource.UploadResourcesForClass, content);
        
        return response;
    }

    public async Task<ResponseDto<bool?>?> ActivateDeactivateResourceMaterial(Guid resourceId)
    {
        var pathParameter = new List<string>()
        {
            resourceId.ToString()
        };
        
        var response = await baseService.DeleteAsync<bool?>(ApiEndpoints.Resource.ActivateDeactivateResourceMaterial, Constants.DeleteType.Patch, pathParameter);
        
        return response;
    }

    public async Task<ResponseDto<bool?>?> RemoveResourceMaterialFromTraining(Guid trainingResourceId)
    {
        var pathParameter = new List<string>()
        {
            trainingResourceId.ToString()
        };
        
        var response = await baseService.DeleteAsync<bool?>(ApiEndpoints.Resource.RemoveResourceMaterialForTraining, Constants.DeleteType.Delete, pathParameter);
        
        return response;
    }

    public async Task<ResponseDto<bool?>?> RemoveResourceMaterialFromClass(Guid classResourceId)
    {
        var pathParameter = new List<string>()
        {
            classResourceId.ToString()
        };
        
        var response = await baseService.DeleteAsync<bool?>(ApiEndpoints.Resource.RemoveResourceMaterialForClass, Constants.DeleteType.Delete, pathParameter);
        
        return response;
    }
    
    public async Task<ResponseDto<bool?>?> DeleteResourceMaterial(Guid resourceId)
    {
        var pathParameter = new List<string>()
        {
            resourceId.ToString()
        };
        
        var response = await baseService.DeleteAsync<bool?>(ApiEndpoints.Resource.DeleteResourceMaterial, Constants.DeleteType.Delete, pathParameter);

        return response;
    }

    public async Task<ResponseDto<bool?>?> DownloadResourceMaterial(Guid resourceId)
    {
        var pathParameter = new List<string>()
        {
            resourceId.ToString()
        };
        
        var result = await baseService.DownloadAsync(ApiEndpoints.Resource.DownloadResourceMaterial, pathParameter);

        if (result is not { content: not null, response: not null })
        {
            return new ResponseDto<bool?>()
            {
                Result = false,
                Message = "Resource material could not be downloaded",
                StatusCode = StatusCode.Status400BadRequest
            };
        }

        var response = result.response;
        
        var content = result.content;
            
        var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";

        var resource =
            await baseService.GetAsync<GetResourceDetailsDto>(ApiEndpoints.Resource.GetResourceById, pathParameter);
        
        await jsRuntime.InvokeVoidAsync("downloadFile", content, resource?.Result?.Title ?? "Resource File", contentType);

        return new ResponseDto<bool?>()
        {
            Result = true,
            Message = "Resource material successfully downloaded.",
            StatusCode = StatusCode.Status200Ok
        };
    }

    public async Task<ResponseDto<string?>?> NavigateToResourceMaterialLink(Guid resourceId)
    {
        var pathParameter = new List<string>()
        {
            resourceId.ToString()
        };
        
        var response = await baseService.GetAsync<string?>(ApiEndpoints.Resource.NavigateToResourceMaterialLink, pathParameter);

        return response;
    }
    
    public async Task<ResponseDto<byte[]?>?> GenerateModuleResourceMaterialQrCode(Guid resourceId)
    {
        var pathParameter = new List<string>()
        {
            resourceId.ToString(),
        };
        
        var response = await baseService.GetAsync<byte[]?>(ApiEndpoints.Resource.GenerateModuleResourceMaterialQrCode, pathParameter);

        return response;
    }
    
    public async Task<ResponseDto<bool?>?> DownloadModuleResourceMaterialQrCode(ResourceGenerateQrCodeDto resourceDownload)
    {
        var resource = await GetResourceById(resourceDownload.ResourceId);

        var jsonRequest = JsonSerializer.Serialize(resourceDownload);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var result = await baseService.DownloadAsync(ApiEndpoints.Resource.DownloadModuleResourceMaterialQrCode, content);

        if (result is not { content: not null, response: not null })
        {
            return new ResponseDto<bool?>()
            {
                Result = false,
                Message = "QR Code for the respective resource material could not be generated.",
                StatusCode = StatusCode.Status400BadRequest
            };
        }

        var response = result.response;
        
        var responseContent = result.content;
            
        var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";

        await jsRuntime.InvokeVoidAsync("downloadFile", responseContent, resource?.Result?.Title ?? "Resource File", contentType);

        return new ResponseDto<bool?>()
        {
            Result = true,
            Message = "QR Code for the respective resource material successfully generated.",
            StatusCode = StatusCode.Status200Ok
        };
    }
}