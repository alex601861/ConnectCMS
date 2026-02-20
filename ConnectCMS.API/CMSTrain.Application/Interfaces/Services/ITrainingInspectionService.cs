using CMSTrain.Application.Common.Service;
using CMSTrain.Application.DTOs.TrainingInspection;

namespace CMSTrain.Application.Interfaces.Services;

public interface ITrainingInspectionService : ITransientService
{
    GetTrainingInspectionDetailsDto GetTrainingInspectionById(Guid trainingInspectionId);

    GetTrainingInspectionDetailsDto GetTrainingInspectionByQuestionnaire(Guid questionnaireId);

    List<GetTrainingInspectionDto> GetAllAssignedTrainingInspections(Guid trainingId, int pageNumber, int pageSize, out int rowCount, string? search = null);

    List<GetTrainingInspectionDto> GetAllAssignedTrainingInspections(Guid trainingId, string? search = null);
    
    List<GetTrainingInspectionDto> GetAllAssignedTrainingInspectionsForCandidateAndClient(Guid trainingId, int pageNumber, int pageSize, out int rowCount, string? search = null);

    List<GetTrainingInspectionDto> GetAllAssignedTrainingInspectionsForCandidateAndClient(Guid trainingId, string? search = null);
    
    List<GetTrainingInspectionDto> GetAllAssignedTrainingInspectionsForTrainingCandidate(Guid trainingCandidateId, int pageNumber, int pageSize, out int rowCount, string? search = null);

    List<GetTrainingInspectionDto> GetAllAssignedTrainingInspectionsForTrainingCandidate(Guid trainingCandidateId, string? search = null);
    
    GetCandidateTrainingInspectionDto GetCandidateTrainingInspectionDetails(Guid trainingInspectionId);

    GetSubordinateTrainingInspectionDto GetSubordinateTrainingInspectionDetails(Guid subordinateId);

    GetCandidateTrainingInspectionDto GetCandidateTrainingInspectionDetailsForTrainingCandidate(Guid trainingCandidateId, Guid trainingInspectionId);

    GetTrainingInspectionQuestionnaireCountDto GetTrainingInspectionQuestionnairesCount(Guid trainingId);

    int GetTrainingInspectionPhaseCounts(Guid trainingInspectionId);

    void AssignTrainingInspections(AssignTrainingInspectionDto trainingInspections);
    
    Task TriggerTrainingInspectionQuestionnaireForSubordinates(Guid trainingInspectionConfigurationId, string recurringJobId);
}