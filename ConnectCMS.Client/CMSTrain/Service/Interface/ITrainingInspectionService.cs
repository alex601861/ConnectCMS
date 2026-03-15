using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Dependency;
using CMSTrain.Client.Models.Requests.TrainingInspection;
using CMSTrain.Client.Models.Responses.TrainingInspection;

namespace CMSTrain.Client.Service.Interface;

public interface ITrainingInspectionService : ITransientService
{
    Task<ResponseDto<GetTrainingInspectionDetailsDto?>?> GetTrainingInspectionById(Guid trainingInspectionId);

    Task<ResponseDto<GetTrainingInspectionDetailsDto?>?> GetTrainingInspectionByQuestionnaire(Guid questionnaireId);

    Task<CollectionDto<GetTrainingInspectionDto>?> GetAllAssignedTrainingInspections(Guid trainingId, int pageNumber, int pageSize, string? search);
    
    Task<ResponseDto<List<GetTrainingInspectionDto>?>?> GetAllAssignedTrainingInspections(Guid trainingId, string? search);
    
    Task<CollectionDto<GetTrainingInspectionDto>?> GetAllAssignedTrainingInspectionsForCandidate(Guid trainingId, int pageNumber, int pageSize, string? search);
    
    Task<ResponseDto<List<GetTrainingInspectionDto>?>?> GetAllAssignedTrainingInspectionsForCandidate(Guid trainingId, string? search);
    
    Task<CollectionDto<GetTrainingInspectionDto>?> GetAllAssignedTrainingInspectionsForClient(Guid trainingId, int pageNumber, int pageSize, string? search);
    
    Task<ResponseDto<List<GetTrainingInspectionDto>?>?> GetAllAssignedTrainingInspectionsForClient(Guid trainingId, string? search);
    
    Task<CollectionDto<GetTrainingInspectionDto>?> GetAllAssignedTrainingInspectionsForTrainingCandidate(Guid trainingCandidateId, int pageNumber, int pageSize, string? search);
    
    Task<ResponseDto<List<GetTrainingInspectionDto>?>?> GetAllAssignedTrainingInspectionsForTrainingCandidate(Guid trainingCandidateId, string? search);
    
    Task<ResponseDto<GetCandidateTrainingInspectionDto?>?> GetCandidateTrainingInspectionDetails(Guid trainingInspectionId);

    Task<ResponseDto<GetSubordinateTrainingInspectionDto?>?> GetSubordinateTrainingInspectionDetails(Guid subordinateId);

    Task<ResponseDto<GetCandidateTrainingInspectionDto?>?> GetCandidateTrainingInspectionDetailsForTrainingCandidate(Guid trainingCandidateId, Guid trainingInspectionId);
    
    Task<ResponseDto<GetTrainingInspectionQuestionnaireCountDto?>?> GetTrainingInspectionQuestionnairesCount(Guid trainingId);

    Task<ResponseDto<int?>?> GetTrainingInspectionPhaseCounts(Guid trainingInspectionId);

    Task<ResponseDto<bool?>?> AssignTrainingInspections(AssignTrainingInspectionDto trainingInspections);
}