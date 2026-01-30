using CMSTrain.Application.Common.Service;
using CMSTrain.Application.DTOs.Resource;

namespace CMSTrain.Application.Interfaces.Services;

public interface IResourceService : ITransientService
{
    ResourceDetailsDto GetResourceById(Guid resourceId);

    ResourceModuleDetailsDto GetTrainingResourceById(Guid trainingResourceId);
    
    ResourceModuleDetailsDto GetClassResourceById(Guid classResourceId);
    
    List<ResourceDetailsDto> GetAllResources(int pageNumber, int pageSize, out int rowCount, string? search = null);

    List<ResourceDetailsDto> GetAllResources(string? search = null);
    
    List<ResourceModuleDetailsDto> GetAllResourcesForTraining(Guid trainingId, string? search = null, bool? isActive = null);

    List<ResourceModuleDetailsDto> GetAllResourcesForTraining(Guid trainingId, int pageNumber, int pageSize, out int rowCount, string? search = null, bool? isActive = null);

    List<ResourceModuleDetailsDto> GetAllResourcesForClass(Guid classId, string? search = null, bool? isActive = null);
    
    List<ResourceModuleDetailsDto> GetAllResourcesForClass(Guid classId, int pageNumber, int pageSize, out int rowCount, string? search = null, bool? isActive = null);

    void ActivateDeactivateResourceForTraining(Guid trainingResourceId);

    void ActivateDeactivateResourceForClass(Guid classResourceId);

    void UploadResources(ResourceUploadDto resource);

    void UpdateResources(UpdateResourceDto resource);

    void UpdateResources(ResourcePostUpdateDto resourcePostUpdate);

    void UploadResourcesPost(ResourcePostDto resource);

    void UploadResourceModule(ResourceModuleUploadDto resource);

    void UploadResourcesForTraining(TrainingResourceUploadDto resources);

    void UploadResourcesForClass(ClassResourceUploadDto resources);

    void ActivateDeactivateResourceMaterial(Guid resourceId);

    void DeleteResourceMaterial(Guid resourceId);

    void RemoveResourceMaterialFromTraining(Guid trainingResourceId);

    void RemoveResourceMaterialFromClass(Guid classResourceId);

    string DownloadResourceMaterial(Guid resourceId);

    string NavigateToResourceMaterialLink(Guid resourceId);
    
    byte[] GenerateModuleResourceMaterialQrCode(Guid moduleId);
}
