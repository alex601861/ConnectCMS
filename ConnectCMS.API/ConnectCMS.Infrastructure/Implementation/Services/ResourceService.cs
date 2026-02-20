using CMSTrain.Helper;
using CMSTrain.Domain.Common;
using CMSTrain.Domain.Entities;
using Microsoft.AspNetCore.Http;
using CMSTrain.Domain.Common.Enum;
using Microsoft.Extensions.Options;
using CMSTrain.Application.Settings;
using CMSTrain.Application.Exceptions;
using CMSTrain.Application.DTOs.Resource;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.Interfaces.Repositories.Base;

namespace CMSTrain.Infrastructure.Implementation.Services;

public class ResourceService(IGenericRepository genericRepository, 
    IOptions<ClientSettings> clientSettings,
    IHttpContextAccessor httpContextAccessor,
    IQrCodeService qrCodeService,
    IFileService fileService) : IResourceService
{
    private const string ResourcesFilePath = Constants.FilePath.ResourcesFilePath;

    private readonly string _baseUrl = clientSettings.Value.BaseUrl.Split(";").FirstOrDefault() 
                                       ?? throw new NotFoundException("The Base URL has not been stabilized and initialized");
    
    public ResourceDetailsDto GetResourceById(Guid resourceId)
    {
        var resource = genericRepository.GetById<Resource>(resourceId)
                       ?? throw new NotFoundException("The following resource file could not be found, please try again with a valid identifier");

        return new ResourceDetailsDto()
        {
            Id = resource.Id,
            Title = resource.Title,
            Tag = resource.Tag,
            Description = resource.Description,
            IsLink = resource.Type is FileType.Link or FileType.Post,
            Link = resource.Type == FileType.Link ? resource.FileUrl : null,
            Type = resource.Type.ToString(),
            UploadedDate = resource.CreatedAt.ToFormattedDateTime()
        };
    }

    public ResourceModuleDetailsDto GetTrainingResourceById(Guid trainingResourceId)
    {
        var trainingResource = genericRepository.GetById<TrainingResources>(trainingResourceId)
            ?? throw new NotFoundException("The following resource has not been allocated to the following training.");
        
        var resource = genericRepository.GetById<Resource>(trainingResource.ResourceId)
                       ?? throw new NotFoundException("The following resource could not be found.");

        var training = genericRepository.GetById<Training>(trainingResource.TrainingId)
                       ?? throw new NotFoundException("The following training could not be found.");
        
        return new ResourceModuleDetailsDto()
        {
            Id = resource.Id,
            Title = resource.Title,
            Tag = resource.Tag,
            Description = resource.Description,
            IsLink = resource.Type is FileType.Link or FileType.Post,
            Link = resource.Type == FileType.Link ? resource.FileUrl : null,
            Type = resource.Type.ToString(),
            UploadedDate = resource.CreatedAt.ToFormattedDateTime(),
            ModuleId = trainingResource.Id,
            DetailId = training.Id,
            AssignedDate = trainingResource.CreatedAt.ToFormattedDateTime(),
            IsActive = trainingResource.IsActive
        };
    }

    public ResourceModuleDetailsDto GetClassResourceById(Guid classResourceId)
    {
        var classResource = genericRepository.GetById<ClassResources>(classResourceId)
                               ?? throw new NotFoundException("The following resource has not been allocated to the following class.");
        
        var resource = genericRepository.GetById<Resource>(classResource.ResourceId)
                       ?? throw new NotFoundException("The following resource could not be found.");
        
        var @class = genericRepository.GetById<Class>(classResource.ClassId)
                       ?? throw new NotFoundException("The following class could not be found.");
        
        return new ResourceModuleDetailsDto()
        {
            Id = resource.Id,
            Title = resource.Title,
            Tag = resource.Tag,
            Description = resource.Description,
            IsLink = resource.Type is FileType.Link or FileType.Post,
            Link = resource.Type == FileType.Link ? resource.FileUrl : null,
            Type = resource.Type.ToString(),
            UploadedDate = resource.CreatedAt.ToFormattedDateTime(),
            ModuleId = classResource.Id,
            DetailId = @class.Id,
            AssignedDate = classResource.CreatedAt.ToFormattedDateTime(),
            IsActive = classResource.IsActive
        };
    }
    
    public List<ResourceDetailsDto> GetAllResources(int pageNumber, int pageSize, out int rowCount, string? search = null)
    {
        var resources = genericRepository.GetPagedResult<Resource>(pageNumber, pageSize, out rowCount, x => string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search.ToLower())).ToList();

        return resources.Select(x => new ResourceDetailsDto()
        {
            Id = x.Id,
            Title = x.Title,
            Tag = x.Tag,
            Description = x.Type == FileType.Post ? x.Tag : x.Description,
            IsLink = x.Type is FileType.Link or FileType.Post,
            Type = x.Type.ToString(),
            Link = x.FileUrl,
            UploadedDate = x.CreatedAt.ToFormattedDateTime(),
        }).ToList();
    }

    public List<ResourceDetailsDto> GetAllResources(string? search = null)
    {
        var resources = genericRepository.Get<Resource>(x => string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search.ToLower())).ToList();

        return resources.Select(x => new ResourceDetailsDto()
        {
            Id = x.Id,
            Title = x.Title,
            Tag = x.Tag,
            Description = x.Type == FileType.Post ? x.Tag : x.Description,
            IsLink = x.Type is FileType.Link or FileType.Post,
            Type = x.Type.ToString(),
            Link = x.FileUrl,
            UploadedDate = x.CreatedAt.ToFormattedDateTime()
        }).ToList();
    }
    
    public List<ResourceModuleDetailsDto> GetAllResourcesForTraining(Guid trainingId, string? search = null, bool? isActive = null)
    {
        var training = genericRepository.GetById<Training>(trainingId)
                       ?? throw new NotFoundException("The following training could not be found.");

        var trainingResources =
            genericRepository.Get<TrainingResources>(x =>
                x.TrainingId == training.Id && (isActive == null || x.IsActive == isActive)).ToList();

        return (from trainingResource in trainingResources
                let resource = genericRepository.GetById<Resource>(trainingResource.ResourceId)
                               ?? throw new NotFoundException("The following resource could not be found.")
                where string.IsNullOrEmpty(search) || resource.Title.ToLower().Contains(search.ToLower())
                select new ResourceModuleDetailsDto
                {
                    Id = resource.Id,
                    Title = resource.Title,
                    Tag = resource.Tag,
                    Description = resource.Description,
                    IsLink = resource.Type is FileType.Link or FileType.Post,
                    Link = resource.Type == FileType.Link ? resource.FileUrl : null,
                    Type = resource.Type.ToString(),
                    UploadedDate = resource.CreatedAt.ToFormattedDateTime(),
                    ModuleId = trainingResource.Id,
                    DetailId = training.Id,
                    AssignedDate = trainingResource.CreatedAt.ToFormattedDateTime(),
                    IsActive = trainingResource.IsActive
                }).ToList();
    }

    public List<ResourceModuleDetailsDto> GetAllResourcesForTraining(Guid trainingId, int pageNumber, int pageSize, out int rowCount, string? search = null, bool? isActive = null)
    {
        var training = genericRepository.GetById<Training>(trainingId)
                       ?? throw new NotFoundException("The following training could not be found.");

        var trainingResources =
            genericRepository.GetPagedResult<TrainingResources>(pageNumber, pageSize, out rowCount, x =>
                x.TrainingId == training.Id && (isActive == null || x.IsActive == isActive)).ToList();

        return (from trainingResource in trainingResources
                let resource = genericRepository.GetById<Resource>(trainingResource.ResourceId)
                               ?? throw new NotFoundException("The following resource could not be found.")
                where string.IsNullOrEmpty(search) || resource.Title.ToLower().Contains(search.ToLower())
                select new ResourceModuleDetailsDto()
                {
                    Id = resource.Id,
                    Title = resource.Title,
                    Tag = resource.Tag,
                    Description = resource.Description,
                    IsLink = resource.Type is FileType.Link or FileType.Post,
                    Link = resource.Type == FileType.Link ? resource.FileUrl : null,
                    ModuleLink = resource.Type == FileType.Image ? resource.FileUrl : null,
                    Type = resource.Type.ToString(),
                    UploadedDate = resource.CreatedAt.ToFormattedDateTime(),
                    ModuleId = trainingResource.Id,
                    DetailId = training.Id,
                    AssignedDate = trainingResource.CreatedAt.ToFormattedDateTime(),
                    IsActive = trainingResource.IsActive
                }).ToList();
    }

    public List<ResourceModuleDetailsDto> GetAllResourcesForClass(Guid classId, string? search = null, bool? isActive = null)
    {
        var @class = genericRepository.GetById<Class>(classId)
                      ?? throw new NotFoundException("The following class could not be found.");

        var classResources =
            genericRepository.Get<ClassResources>(x =>
                x.ClassId == @class.Id && (isActive == null || x.IsActive == isActive)).ToList();

        return (from classResource in classResources
                let resource = genericRepository.GetById<Resource>(classResource.ResourceId)
                               ?? throw new NotFoundException("The following resource could not be found.")
                where string.IsNullOrEmpty(search) || resource.Title.ToLower().Contains(search.ToLower())
                select new ResourceModuleDetailsDto()
                {
                    Id = resource.Id,
                    Title = resource.Title,
                    Tag = resource.Tag,
                    Description = resource.Description,
                    IsLink = resource.Type is FileType.Link or FileType.Post,
                    Link = resource.Type == FileType.Link ? resource.FileUrl : null,
                    Type = resource.Type.ToString(),
                    UploadedDate = resource.CreatedAt.ToFormattedDateTime(),
                    ModuleId = classResource.Id,
                    DetailId = @class.Id,
                    AssignedDate = classResource.CreatedAt.ToFormattedDateTime(),
                    IsActive = classResource.IsActive
                }).ToList();
    }

    public List<ResourceModuleDetailsDto> GetAllResourcesForClass(Guid classId, int pageNumber, int pageSize, out int rowCount, string? search = null, bool? isActive = null)
    {
        var @class = genericRepository.GetById<Class>(classId)
                     ?? throw new NotFoundException("The following class could not be found.");

        var classResources =
            genericRepository.GetPagedResult<ClassResources>(pageNumber, pageSize, out rowCount, x =>
                x.ClassId == @class.Id && (isActive == null || x.IsActive == isActive)).ToList();

        return (from classResource in classResources
                let resource = genericRepository.GetById<Resource>(classResource.ResourceId)
                               ?? throw new NotFoundException("The following resource could not be found.")
                where string.IsNullOrEmpty(search) || resource.Title.ToLower().Contains(search.ToLower())
                select new ResourceModuleDetailsDto()
                {
                    Id = resource.Id,
                    Title = resource.Title,
                    Tag = resource.Tag,
                    Description = resource.Type == FileType.Post ? resource.Tag ?? "" : resource.Description,
                    IsLink = resource.Type is FileType.Link or FileType.Post,
                    Link = resource.Type == FileType.Link ? resource.FileUrl : null,
                    Type = resource.Type.ToString(),
                    UploadedDate = resource.CreatedAt.ToFormattedDateTime(),
                    ModuleId = classResource.Id,
                    DetailId = @class.Id,
                    AssignedDate = classResource.CreatedAt.ToFormattedDateTime(),
                    IsActive = classResource.IsActive
                }).ToList();
    }

    public void ActivateDeactivateResourceForTraining(Guid trainingResourceId)
    {
        var trainingResource = genericRepository.GetById<TrainingResources>(trainingResourceId)
                               ?? throw new NotFoundException("The following resource has not been allocated to the respective training.");
        
        trainingResource.IsActive = !trainingResource.IsActive;
        
        genericRepository.Update(trainingResource);
    }

    public void ActivateDeactivateResourceForClass(Guid classResourceId)
    {
        var classResource = genericRepository.GetById<ClassResources>(classResourceId)
                            ?? throw new NotFoundException("The following resource has not been allocated to the respective class.");
        
        classResource.IsActive = !classResource.IsActive;
        
        genericRepository.Update(classResource);
    }
    
    public void UploadResources(ResourceUploadDto resource)
    {
        var fileType = resource.IsLink ?
            FileType.Link :
            ExtensionMethod.GetFileType(resource.ResourceFile!);

        var resourceModel = new Resource
        {
            Title = resource.Title,
            Description = resource.Description,
            Type = fileType ?? FileType.Documents,
            Tag = ""
        };

        if (resource.IsLink)
        {
            resourceModel.FileName = "";
            resourceModel.FileUrl = resource.Link ??
                                    throw new BadRequestException("The following resource could not be uploaded",
                                        ["Please upload a link before submitting your request."]);
        }
        else
        {
            resourceModel.FileName = resource.ResourceFile?.FileName ??
                                     throw new BadRequestException("The following resource could not be uploaded",
                                         ["Please upload a valid file before submitting your request."]);
            resourceModel.FileUrl = fileService.UploadDocument(resource.ResourceFile, ResourcesFilePath);
        }

        genericRepository.Insert(resourceModel);
    }

    public void UpdateResources(UpdateResourceDto resource)
    {
        var resourceModel = genericRepository.GetById<Resource>(resource.Id)
                            ?? throw new NotFoundException("The following resource was not found.");
        
        var fileType = resource.IsLink ?
            FileType.Link :
            resource.ResourceFile != null ?
                ExtensionMethod.GetFileType(resource.ResourceFile) :
                null;

        resourceModel.Title = resource.Title;
        resourceModel.Description = resource.Description;
        
        if (resource.IsLink)
        {
            resourceModel.FileName = "";
            resourceModel.FileUrl = resource.Link ??
                                    throw new BadRequestException("The following resource could not be uploaded",
                                        ["Please upload a link before submitting your request."]);
        }
        else
        {
            if (resource.ResourceFile != null)
            {
                var resourcePath = Path.Combine(ResourcesFilePath, resourceModel.FileUrl);

                fileService.DeleteFile(resourcePath);
                
                resourceModel.FileName = resource.ResourceFile?.FileName ??
                                         throw new BadRequestException("The following resource could not be uploaded",
                                             ["Please upload a valid file before submitting your request."]);
                
                resourceModel.FileUrl = fileService.UploadDocument(resource.ResourceFile, ResourcesFilePath);
            }
        }
        
        if (fileType != null)
        {
            resourceModel.Type = (FileType)fileType;
        }
        
        genericRepository.Update(resourceModel);
    }

    public void UpdateResources(ResourcePostUpdateDto resourcePostUpdate)
    {
        var resourceModel = genericRepository.GetById<Resource>(resourcePostUpdate.Id)
                            ?? throw new NotFoundException("The following resource was not found.");
        
        resourceModel.Title = resourcePostUpdate.Title;
        resourceModel.Description = resourcePostUpdate.Description;
        resourceModel.Tag = resourcePostUpdate.Tag;
        
        genericRepository.Update(resourceModel);
    }

    public void UploadResourcesPost(ResourcePostDto resource)
    {
        var resourceModel = new Resource
        {
            Title = resource.Title,
            Description = resource.Description,
            Tag = resource.Tag,
            Type = FileType.Post,
            FileUrl = string.Empty,
            FileName = string.Empty,
        };

        genericRepository.Insert(resourceModel);
    }
    
    public void UploadResourceModule(ResourceModuleUploadDto resource)
    {
        var resourceMaterial = resource.Resource;

        var fileType = resourceMaterial.IsLink ?
            FileType.Link :
            ExtensionMethod.GetFileType(resourceMaterial.ResourceFile!);

        var resourceModel = new Resource
        {
            Title = resourceMaterial.Title,
            Description = resourceMaterial.Description,
            Type = fileType ?? FileType.Documents,
            Tag = ""
        };

        if (resourceMaterial.IsLink)
        {
            resourceModel.FileName = "";
            resourceModel.FileUrl = resourceMaterial.Link ??
                                    throw new BadRequestException("The following resource could not be uploaded",
                                        ["Please upload a link before submitting your request."]);
        }
        else
        {
            resourceModel.FileName = resourceMaterial.ResourceFile?.FileName ??
                                     throw new BadRequestException("The following resource could not be uploaded",
                                         ["Please upload a valid file before submitting your request."]);
            
            resourceModel.FileUrl = fileService.UploadDocument(resourceMaterial.ResourceFile, ResourcesFilePath);
        }

        var resourceId = genericRepository.Insert(resourceModel);

        if (resource.IsMaterialForTraining)
        {
            var training = genericRepository.GetById<Training>(resource.ModuleId)
                ?? throw new NotFoundException("The following training could not be found.");

            var trainingResource = new TrainingResources()
            {
                TrainingId = training.Id,
                ResourceId = resourceId
            };

            genericRepository.Insert(trainingResource);
        }
        else
        {
            var @class = genericRepository.GetById<Class>(resource.ModuleId)
                           ?? throw new NotFoundException("The following class could not be found.");

            var classResource = new ClassResources()
            {
                ClassId = @class.Id,
                ResourceId = resourceId
            };

            genericRepository.Insert(classResource);
        }
    }

    public void UploadResourcesForTraining(TrainingResourceUploadDto resource)
    {
        var training = genericRepository.GetById<Training>(resource.TrainingId)
                       ?? throw new NotFoundException("The following training could not be found.");

        var trainingResources =
            genericRepository.Get<TrainingResources>(x =>
                x.TrainingId == training.Id).ToList();

        var deletableTrainingResourceMaterials = trainingResources
            .Where(x => !resource.ResourceIds.Contains(x.ResourceId))
            .ToList();

        genericRepository.RemoveMultipleEntity(deletableTrainingResourceMaterials);

        foreach (var resourceId in resource.ResourceIds)
        {
            var resourceModel = genericRepository.GetById<Resource>(resourceId)
                                ?? throw new NotFoundException("The following resource could not be found.");

            if (trainingResources.Any(x => x.ResourceId == resourceModel.Id))
                continue;

            var trainingResource = new TrainingResources()
            {
                TrainingId = training.Id,
                ResourceId = resourceModel.Id
            };

            genericRepository.Insert(trainingResource);
        }
        
        var @class = genericRepository.GetFirstOrDefault<Class>(x => 
            x.TrainingId == training.Id && x.IsDefaultClass != null && x.IsDefaultClass.Value == true);

        if (@class == null) return;

        var classResources = new ClassResourceUploadDto()
        {
            ClassId = @class.Id,
            ResourceIds = resource.ResourceIds
        };
        
        UploadResourcesForClass(classResources);
    }

    public void UploadResourcesForClass(ClassResourceUploadDto resource)
    {
        var @class = genericRepository.GetById<Class>(resource.ClassId)
                       ?? throw new NotFoundException("The following class could not be found.");

        var trainingResources =
            genericRepository.Get<ClassResources>(x =>
                x.ClassId == @class.Id).ToList();

        var deletableResourceMaterials = trainingResources
            .Where(x => !resource.ResourceIds.Contains(x.ResourceId))
            .ToList();

        genericRepository.RemoveMultipleEntity(deletableResourceMaterials);

        foreach (var resourceId in resource.ResourceIds)
        {
            var resourceModel = genericRepository.GetById<Resource>(resourceId)
                                ?? throw new NotFoundException("The following resource could not be found.");

            if (trainingResources.Any(x => x.ResourceId == resourceModel.Id))
                continue;

            var trainingResource = new ClassResources()
            {
                ClassId = @class.Id,
                ResourceId = resourceModel.Id
            };

            genericRepository.Insert(trainingResource);
        }
    }

    public void ActivateDeactivateResourceMaterial(Guid resourceId)
    {
        var resource = genericRepository.GetById<Resource>(resourceId)
                       ?? throw new NotFoundException("The following resource file could not be found, please try again with a valid identifier");

        resource.IsActive = !resource.IsActive;

        genericRepository.Update(resource);
    }

    public void DeleteResourceMaterial(Guid resourceId)
    {
        var resource = genericRepository.GetById<Resource>(resourceId)
                       ?? throw new NotFoundException("The following resource file could not be found, please try again with a valid identifier");

        var trainingResources = genericRepository.Get<TrainingResources>(x =>
            x.ResourceId == resource.Id).ToList();

        var classResources = genericRepository.Get<ClassResources>(x =>
            x.ResourceId == resource.Id).ToList();

        if (trainingResources.Count != 0)
        {
            genericRepository.RemoveMultipleEntity(trainingResources);
        }
        
        if (classResources.Count != 0)
        {
            genericRepository.RemoveMultipleEntity(classResources);
        }

        var resourcePath = Path.Combine(ResourcesFilePath, resource.FileUrl);

        fileService.DeleteFile(resourcePath);

        genericRepository.Delete(resource);
    }

    public void RemoveResourceMaterialFromTraining(Guid trainingResourceId)
    {
        var trainingResource = genericRepository.GetById<TrainingResources>(trainingResourceId) ??
                               throw new NotFoundException(
                                   "The following resource has not been allocated to the respective training.");

        genericRepository.Delete(trainingResource);
    }

    public void RemoveResourceMaterialFromClass(Guid classResourceId)
    {
        var classResource = genericRepository.GetById<ClassResources>(classResourceId) ??
                               throw new NotFoundException(
                                   "The following resource has not been allocated to the respective class.");

        genericRepository.Delete(classResource);
    }
    
    public string DownloadResourceMaterial(Guid resourceId)
    {
        var resource = genericRepository.GetById<Resource>(resourceId);

        if (resource == null || resource.Type == FileType.Link)
            throw new NotFoundException("The following resource identifier does not allocate a valid material, please try again.");

        var resourcePath = Path.Combine(ResourcesFilePath, resource.FileUrl);

        var filePath = fileService.FileExistPath(resourcePath);

        return !string.IsNullOrEmpty(filePath) ? filePath : string.Empty;
    }

    public string NavigateToResourceMaterialLink(Guid resourceId)
    {
        var resource = genericRepository.GetById<Resource>(resourceId);

        if (resource is not { Type: FileType.Image })
            throw new NotFoundException("The following resource identifier does not allocate a valid material, please try again.");

        var request = httpContextAccessor.HttpContext?.Request;
                
        var baseUrl = $"{request?.Scheme}://{request?.Host}{request?.PathBase}";
                
        return Path.Combine(baseUrl, ResourcesFilePath, resource.FileUrl);
    }
    
    public byte[] GenerateModuleResourceMaterialQrCode(Guid resourceId)
    {
        var resource = genericRepository.GetById<Resource>(resourceId)
                       ?? throw new NotFoundException("The following resource file could not be found, please try again with a valid identifier");
        
        return GenerateResourceMaterialQrCode(resource.Id);
    }
    
    private byte[] GenerateResourceMaterialQrCode(Guid resourceId)
    {
        var resourceNavigation = $"{_baseUrl}/{Constants.Navigation.ResourceMaterialDownload}/{resourceId}";
        
        var base64QrCode = qrCodeService.GenerateQrCode(resourceNavigation);
        
        return Convert.FromBase64String(base64QrCode);
    }
}
