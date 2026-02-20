using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Dependency;
using CMSTrain.Client.Models.Requests.Inspection;
using CMSTrain.Client.Models.Responses.Inspection;

namespace CMSTrain.Client.Service.Interface;

public interface IInspectionService : ITransientService
{
    Task<ResponseDto<GetInspectionDto?>?> GetInspectionById(Guid inspectionId);

    Task<ResponseDto<GetInspectionDto?>?> GetInspectionByType(InspectionType inspectionType);

    Task<CollectionDto<GetInspectionDto>?> GetAllInspections(int pageNumber, int pageSize, string? search = null, bool? isActive = null);

    Task<ResponseDto<List<GetInspectionDto>?>?> GetAllInspections(string? search = null, bool? isActive = null);
    
    Task<CollectionDto<GetInspectionDto>?> GetAllAvailableTrainingInspections(int pageNumber, int pageSize);
    
    Task<ResponseDto<List<GetInspectionDto>?>?> GetAllAvailableTrainingInspections();

    Task<CollectionDto<GetInspectionDto>?> GetAllAssignedTrainingInspections(Guid trainingId, int pageNumber, int pageSize, string? search);

    Task<ResponseDto<List<GetInspectionDto>?>?> GetAllAssignedTrainingInspections(Guid trainingId);

    Task<ResponseDto<bool?>?> InsertInspection(CreateInspectionDto inspection);

    Task<ResponseDto<bool?>?> UpdateInspection(UpdateInspectionDto inspection);

    Task<ResponseDto<bool?>?> ActivateDeactivateInspection(Guid inspectionId);

    Task<ResponseDto<bool?>?> UploadInspectionQuestionnaires(UploadInspectionQuestionnaireDto inspectionQuestionnaires);
}