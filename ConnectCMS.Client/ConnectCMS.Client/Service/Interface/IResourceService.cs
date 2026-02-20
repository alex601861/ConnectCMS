using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Models.Requests.Resource;
using CMSTrain.Client.Models.Responses.Resource;
using CMSTrain.Client.Service.Dependency;

namespace CMSTrain.Client.Service.Interface;

public interface IResourceService : ITransientService
{
    Task<ResponseDto<GetResourceDetailsDto?>?> GetResourceById(Guid resourceId);

    Task<ResponseDto<GetResourceModuleDetailsDto?>?> GetTrainingResourceById(Guid trainingResourceId);

    Task<ResponseDto<GetResourceModuleDetailsDto?>?> GetClassResourceById(Guid classResourceId);

    Task<CollectionDto<GetResourceDetailsDto>?> GetAllResources(int pageNumber, int pageSize, string? search = null);

    Task<ResponseDto<List<GetResourceDetailsDto>?>?> GetAllResources(string? search = null);

    Task<CollectionDto<GetResourceModuleDetailsDto>?> GetAllResourcesForTraining(Guid trainingId, int pageNumber, int pageSize, string? search = null, bool? isActive = null);

    Task<ResponseDto<List<GetResourceModuleDetailsDto>?>?> GetAllResourcesForTraining(Guid trainingId, string? search = null, bool? isActive = null);

    Task<CollectionDto<GetResourceModuleDetailsDto>?> GetAllResourcesForClass(Guid classId, int pageNumber, int pageSize, string? search = null, bool? isActive = null);

    Task<ResponseDto<List<GetResourceModuleDetailsDto>?>?> GetAllResourcesForClass(Guid classId, string? search = null, bool? isActive = null);

    Task<ResponseDto<bool?>?> ActivateDeactivateResourceForTraining(Guid trainingResourceId);

    Task<ResponseDto<bool?>?> ActivateDeactivateResourceForClass(Guid classResourceId);

    Task<ResponseDto<bool?>?> UploadResources(ResourceUploadDto resource);

    Task<ResponseDto<bool?>?> UploadResourcesPost(ResourcePostDto resource);

    Task<ResponseDto<bool?>?> UpdateResource(UpdateResourceDto resource);

    Task<ResponseDto<bool?>?> UpdateResource(ResourcePostUpdateDto resourcePostUpdate);

    Task<ResponseDto<bool?>?> UploadResourceModule(ResourceModuleUploadDto resource);

    Task<ResponseDto<bool?>?> UploadResourcesForTraining(TrainingResourceUploadDto resource);
    
    Task<ResponseDto<bool?>?> UploadResourcesForClass(ClassResourceUploadDto resource);

    Task<ResponseDto<bool?>?> ActivateDeactivateResourceMaterial(Guid resourceId);

    Task<ResponseDto<bool?>?> DeleteResourceMaterial(Guid resourceId);

    Task<ResponseDto<bool?>?> RemoveResourceMaterialFromTraining(Guid trainingResourceId);

    Task<ResponseDto<bool?>?> RemoveResourceMaterialFromClass(Guid classResourceId);

    Task<ResponseDto<bool?>?> DownloadResourceMaterial(Guid resourceId);

    Task<ResponseDto<string?>?> NavigateToResourceMaterialLink(Guid resourceId);
    
    Task<ResponseDto<byte[]?>?> GenerateModuleResourceMaterialQrCode(Guid resourceId);

    Task<ResponseDto<bool?>?> DownloadModuleResourceMaterialQrCode(ResourceGenerateQrCodeDto resourceDownload);
}