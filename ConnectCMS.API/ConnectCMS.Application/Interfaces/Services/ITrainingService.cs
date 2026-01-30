using CMSTrain.Application.Common.Service;
using CMSTrain.Application.DTOs.Count;
using CMSTrain.Application.DTOs.Organization;
using CMSTrain.Application.DTOs.Training;
using CMSTrain.Application.DTOs.TrainingCandidate;

namespace CMSTrain.Application.Interfaces.Services;

public interface ITrainingService : ITransientService
{
    List<GetTrainingDto> GetAllTrainings(int statusAction, int pageNumber, int pageSize, out int rowCount, string? search, bool? isActive);
    
    List<GetTrainingDto> GetAllTrainings(int statusAction, string? search, bool? isActive);

    AssignedTrainingCountDto GetAvailableTrainingsCount();
    
    GetTrainingDto GetTrainingById(Guid id);

    GetTrainingDto GetTrainingDetailsByInspection(Guid trainingInspectionId);

    GetTrainingDto GetTrainingDetailsByQuestionnaire(Guid questionnaireId);
    
    TrainingModuleCountDto GetTrainingModuleCount(bool? isActive = null);

    TrainingDetailsCountDto GetTrainingDetailsCount(Guid trainingId);
    
    List<GetOrganizationDto> GetAllAssignedClientOrganizations(Guid trainingId);
    
    void InsertTraining(CreateTrainingDto training);

    void UpdateTraining(UpdateTrainingDto training);

    void ActivateDeactivateTraining(Guid id);
}
