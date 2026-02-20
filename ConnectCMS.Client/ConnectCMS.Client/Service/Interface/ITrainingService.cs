using CMSTrain.Client.Models.Base;
using CMSTrain.Application.DTOs.Count;
using CMSTrain.Client.Service.Dependency;
using CMSTrain.Client.Models.Responses.Count;
using CMSTrain.Client.Models.Requests.Training;
using CMSTrain.Client.Models.Responses.Training;
using CMSTrain.Client.Models.Responses.ClassTrainers;
using CMSTrain.Client.Models.Responses.Organization;
using CMSTrain.Client.Models.Responses.TrainingCandidate;

namespace CMSTrain.Client.Service.Interface;

public interface ITrainingService : ITransientService
{
    #region Admin
    Task<CollectionDto<GetTrainingDto>?> GetAllTrainings(int statusAction, int pageNumber, int pageSize, string? search = null, bool? isActive = null);
    
    Task<ResponseDto<List<GetTrainingDto>?>?> GetAllTrainings(int statusAction, string? search = null, bool? isActive = null);

    Task<ResponseDto<AssignedTrainingCountDto?>?> GetAvailableTrainingsCount();

    Task<ResponseDto<GetTrainingDto?>?> GetTrainingById(Guid trainingId);

    Task<ResponseDto<TrainingModuleCountDto?>?> GetAllTrainingModuleCount(bool? isActive = null);

    Task<ResponseDto<TrainingDetailsCountDto?>?> GetTrainingDetailsCount(Guid trainingId);

    Task<ResponseDto<List<GetOrganizationDto>?>?> GetAllAssignedClientOrganizations(Guid trainingId);

    Task<ResponseDto<bool?>?> InsertTraining(CreateTrainingDto training);
    
    Task<ResponseDto<bool?>?> UpdateTraining(UpdateTrainingDto training);

    Task<ResponseDto<bool?>?> ActivateDeactivateTraining(Guid trainingId);
    #endregion

    #region Trainers
    Task<CollectionDto<GetTrainingDto>?> GetAllTrainingsForTrainer(int pageNumber, int pageSize, string? search = null);

    Task<ResponseDto<List<GetTrainingDto>?>?> GetAllTrainingsForTrainer();

    Task<ResponseDto<AvailableTrainingCountDto?>?> GetAllAvailableTrainingCountForTrainers();
    
    Task<CollectionDto<GetAssignedTrainingsDto>?> GetAllAssignedTrainingsForTrainers(int requestAction, int pageNumber, int pageSize, string? search = null);

    Task<ResponseDto<List<GetAssignedTrainingsDto>?>?> GetAllAssignedTrainingsForTrainers(int requestAction);

    Task<ResponseDto<AssignedTrainingCountDto?>?> GetAllAssignedTrainingCountForTrainers();
    #endregion

    #region Client
    Task<CollectionDto<GetAllTrainingsForClient>?> GetAllTrainingsForClient(int requestAction, int pageNumber, int pageSize, string? search = null);
    
    Task<ResponseDto<List<GetAllTrainingsForClient>?>?> GetAllTrainingsForClient(int requestAction);

    Task<ResponseDto<AvailableTrainingCountDto?>?> GetAllAvailableTrainingCountsForClient();

    Task<CollectionDto<GetAllTrainingsForClient>?> GetAllAssignedTrainingsForClient(int requestAction, int pageNumber, int pageSize, string? search = null);

    Task<ResponseDto<List<GetAllTrainingsForClient>?>?> GetAllAssignedTrainingsForClient(int requestAction);

    Task<ResponseDto<AssignedTrainingCountDto?>?> GetAllAssignedTrainingCountsForClient();

    Task<ResponseDto<TrainingDetailsCountDto?>?> GetTrainingDetailsCountForClient(Guid trainingId);
    #endregion

    #region Candidate
    Task<CollectionDto<GetAllTrainingsForCandidate>?> GetAllTrainingsForCandidate(int requestAction, int pageNumber, int pageSize, string? search = null);

    Task<ResponseDto<List<GetAllTrainingsForCandidate>?>?> GetAllTrainingsForCandidate(int requestAction);

    Task<ResponseDto<AvailableTrainingCountDto?>?> GetAvailableTrainingCountsForCandidate();

    Task<CollectionDto<GetAllTrainingsForCandidate>?> GetAllAssignedTrainingsForCandidate(int requestAction, int pageNumber, int pageSize, string? search = null);
    
    Task<ResponseDto<List<GetAllTrainingsForCandidate>?>?> GetAllAssignedTrainingsForCandidate(int requestAction);

    Task<ResponseDto<AssignedTrainingCountDto?>?> GetAssignedTrainingCountsForCandidate();

    Task<ResponseDto<TrainingDetailsCountDto?>?> GetTrainingDetailsCountForCandidate(Guid trainingId);
    #endregion

    #region Generic Navigation
    Task<ResponseDto<GetTrainingDto?>?> GetTrainingDetailsByInspection(Guid trainingInspectionId);
    
    Task<ResponseDto<GetTrainingDto?>?> GetTrainingDetailsByQuestionnaire(Guid questionnaireId);
    #endregion
}