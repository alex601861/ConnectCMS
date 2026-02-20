using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using CMSTrain.Application.DTOs.Resource;
using CMSTrain.Application.Common.Response;
using CMSTrain.Application.Interfaces.Services;

namespace CMSTrain.Controllers;

[Route("api/resource")]
public class ResourceController(IResourceService resourceService) : BaseController<ResourceController>
{
    [HttpGet("details/{resourceId:guid}")]
    public IActionResult GetResourceById(Guid resourceId)
    {
        var result = resourceService.GetResourceById(resourceId);

        return Ok(new ResponseDto<ResourceDetailsDto>()
        {
            Result = result,
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Resource successfully fetched"
        });
    }
    
    [HttpGet("training/details/{trainingResourceId:guid}")]
    public IActionResult GetTrainingResourceById(Guid trainingResourceId)
    {
        var result = resourceService.GetTrainingResourceById(trainingResourceId);

        return Ok(new ResponseDto<ResourceModuleDetailsDto>()
        {
            Result = result,
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Resource successfully fetched"
        });
    }
    
    [HttpGet("class/details/{classResourceId:guid}")]
    public IActionResult GetClassResourceById(Guid classResourceId)
    {
        var result = resourceService.GetClassResourceById(classResourceId);

        return Ok(new ResponseDto<ResourceModuleDetailsDto>()
        {
            Result = result,
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Resource successfully fetched"
        });
    }
    
    [HttpGet]
    public IActionResult GetAllResources(int pageNumber, int pageSize, string? search)
    {
        var result = resourceService.GetAllResources(pageNumber, pageSize, out var rowCount, search);

        return Ok(new CollectionDto<ResourceDetailsDto>(result, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Resources successfully retrieved."
        });
    }
    
    [HttpGet("list")]
    public IActionResult GetAllResources(string? search)
    {
        var result = resourceService.GetAllResources(search);

        return Ok(new ResponseDto<List<ResourceDetailsDto>>()
        {
            Result = result,
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Resource successfully fetched"
        });
    }
    
    [HttpGet("training/{trainingId:guid}")]
    public IActionResult GetAllResourcesForTraining(Guid trainingId, int pageNumber, int pageSize, string? search, bool? isActive)
    {
        var result = resourceService.GetAllResourcesForTraining(trainingId, pageNumber, pageSize, out var rowCount, search, isActive);

        return Ok(new CollectionDto<ResourceModuleDetailsDto>(result, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Organization successfully retrieved."
        });
    }
    
    [HttpGet("training/list/{trainingId:guid}")]
    public IActionResult GetAllResourcesForTraining(Guid trainingId, string? search, bool? isActive)
    {
        var result = resourceService.GetAllResourcesForTraining(trainingId, search, isActive);

        return Ok(new ResponseDto<List<ResourceModuleDetailsDto>>()
        {
            Message = "Resource material successfully retrieved.",
            Result = result,
            StatusCode = (int)HttpStatusCode.OK,
        });
    }
    
    [HttpGet("class/{classId:guid}")]
    public IActionResult GetAllResourcesForClass(Guid classId, int pageNumber, int pageSize, string? search, bool? isActive)
    {
        var result = resourceService.GetAllResourcesForClass(classId, pageNumber, pageSize, out var rowCount, search, isActive);

        return Ok(new CollectionDto<ResourceModuleDetailsDto>(result, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Organization successfully retrieved."
        });
    }
    
    [HttpGet("class/list/{classId:guid}")]
    public IActionResult GetAllResourcesForClass(Guid classId, string? search, bool? isActive)
    {
        var result = resourceService.GetAllResourcesForClass(classId, search, isActive);

        return Ok(new ResponseDto<List<ResourceModuleDetailsDto>>()
        {
            Message = "Resource material successfully retrieved.",
            Result = result,
            StatusCode = (int)HttpStatusCode.OK,
        });
    }

    [HttpPatch("training/{trainingId:guid}")]
    public IActionResult ActivateDeactivateResourceForTraining(Guid trainingId)
    {
        resourceService.ActivateDeactivateResourceForTraining(trainingId);

        return Ok(new ResponseDto<bool>()
        {
            Message = "Resource material's status successfully modified.",
            Result = true,
            StatusCode = (int)HttpStatusCode.OK,
        });
    }
    
    [HttpPatch("class/{classId:guid}")]
    public IActionResult ActivateDeactivateResourceForClass(Guid classId)
    {
        resourceService.ActivateDeactivateResourceForClass(classId);

        return Ok(new ResponseDto<bool>()
        {
            Message = "Resource material's status successfully modified.",
            Result = true,
            StatusCode = (int)HttpStatusCode.OK,
        });
    }
    
    [HttpPost]
    public IActionResult UploadResources([FromForm] ResourceUploadDto resource)
    {
        resourceService.UploadResources(resource);

        return Ok(new ResponseDto<bool>()
        {
            Message = "Resource material successfully uploaded.",
            Result = true,
            StatusCode = (int)HttpStatusCode.OK,
        });
    }
    
    [HttpPost("post")]
    public IActionResult UploadResourcesPost([FromBody] ResourcePostDto resource)
    {
        resourceService.UploadResourcesPost(resource);

        return Ok(new ResponseDto<bool>()
        {
            Message = "Resource material successfully uploaded.",
            Result = true,
            StatusCode = (int)HttpStatusCode.OK,
        });
    }

    [HttpPost("module")]
    public IActionResult UploadResourceModule([FromForm] ResourceModuleUploadDto resource)
    {
        resourceService.UploadResourceModule(resource);

        return Ok(new ResponseDto<bool>()
        {
            Message = "Resource material successfully uploaded.",
            Result = true,
            StatusCode = (int)HttpStatusCode.OK,
        });
    }
    
    [HttpPost("training")]
    public IActionResult UploadResourcesForTraining(TrainingResourceUploadDto resources)
    {
        resourceService.UploadResourcesForTraining(resources);

        return Ok(new ResponseDto<bool>()
        {
            Message = "File successfully uploaded.",
            Result = true,
            StatusCode = (int)HttpStatusCode.OK,
        });
    }
    
    [HttpPost("class")]
    public IActionResult UploadResourcesForClass(ClassResourceUploadDto resources)
    {
        resourceService.UploadResourcesForClass(resources);
        
        return Ok(new ResponseDto<bool>()
        {
            Message = "File successfully uploaded.",
            Result = true,
            StatusCode = (int)HttpStatusCode.OK,
        });
    }
    
    [HttpPut]
    public IActionResult UpdateResources([FromForm] UpdateResourceDto resource)
    {
        resourceService.UpdateResources(resource);
        
        return Ok(new ResponseDto<bool>()
        {
            Message = "Resource successfully updated.",
            Result = true,
            StatusCode = (int)HttpStatusCode.OK,
        });
    }
    
    [HttpPut("post")]
    public IActionResult UpdateResources([FromBody] ResourcePostUpdateDto resourcePostUpdate)
    {
        resourceService.UpdateResources(resourcePostUpdate);
        
        return Ok(new ResponseDto<bool>()
        {
            Message = "Resource successfully updated.",
            Result = true,
            StatusCode = (int)HttpStatusCode.OK,
        });
    }
    
    [HttpPatch("{resourceId:guid}")]
    public IActionResult ActivateDeactivateResourceMaterial(Guid resourceId)
    {
        resourceService.ActivateDeactivateResourceMaterial(resourceId);

        return Ok(new ResponseDto<bool>()
        {
            Message = "Resource material's status successfully modified.",
            Result = true,
            StatusCode = (int)HttpStatusCode.OK,
        });
    }
    
    [HttpDelete("{resourceId:guid}")]
    public IActionResult DeleteResourceMaterial(Guid resourceId)
    {
        resourceService.DeleteResourceMaterial(resourceId);

        return Ok(new ResponseDto<bool>()
        {
            Message = "Resource material successfully deleted.",
            Result = true,
            StatusCode = (int)HttpStatusCode.OK,
        });
    }

    [HttpDelete("training/{trainingResourceId:guid}")]
    public IActionResult RemoveResourceMaterialFromTraining(Guid trainingResourceId)
    {
        resourceService.RemoveResourceMaterialFromTraining(trainingResourceId);

        return Ok(new ResponseDto<bool>()
        {
            Message = "Resource material successfully deleted.",
            Result = true,
            StatusCode = (int)HttpStatusCode.OK,
        });
    }
    
    [HttpDelete("class/{classResourceId:guid}")]
    public IActionResult RemoveResourceMaterialFromClass(Guid classResourceId)
    {
        resourceService.RemoveResourceMaterialFromClass(classResourceId);

        return Ok(new ResponseDto<bool>()
        {
            Message = "Resource material successfully deleted.",
            Result = true,
            StatusCode = (int)HttpStatusCode.OK,
        });
    }
    
    [HttpGet("download/{resourceId:guid}")]
    public IActionResult DownloadResourceMaterial(Guid resourceId)
    {
        var filePath = resourceService.DownloadResourceMaterial(resourceId);

        if (string.IsNullOrEmpty(filePath))
        {
            return NotFound();
        }

        var fileName = Path.GetFileName(filePath);

        var contentType = GetContentType(fileName);

        return PhysicalFile(filePath, contentType, fileName);
    }
    
    [HttpGet("navigate/{resourceId:guid}")]
    public IActionResult NavigateToResourceMaterialLink(Guid resourceId)
    {
        var result = resourceService.NavigateToResourceMaterialLink(resourceId);

        return Ok(new ResponseDto<string>()
        {
            Result = result,
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Resource Material QR Code Successfully Generated"
        });
    }
    
    [HttpGet("qr-code/{resourceId:guid}")]
    public IActionResult GenerateModuleResourceMaterialQrCode(Guid resourceId)
    {
        var result = resourceService.GenerateModuleResourceMaterialQrCode(resourceId);

        return Ok(new ResponseDto<byte[]>()
        {
            Result = result,
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Resource Material QR Code Successfully Generated"
        });
    }

    [HttpPost("qr-code")]
    public async Task<IActionResult> DownloadModuleResourceMaterialQrCode(ResourceDownloadQrCodeDto resourceDownload)
    {
        var resource = resourceService.GetResourceById(resourceDownload.ResourceId); 
        
        var qrCodeBytes = resourceService.GenerateModuleResourceMaterialQrCode(resource.Id);
        
        using var image = Image.Load(qrCodeBytes);
        
        using var ms = new MemoryStream();
        
        await image.SaveAsync(ms, new PngEncoder());
        
        ms.Position = 0;

        return File(ms.ToArray(), "image/png", $"QRCode_{resource.Title}.png");
    }
}
