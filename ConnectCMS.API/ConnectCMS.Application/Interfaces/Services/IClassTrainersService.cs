using CMSTrain.Application.DTOs.Count;
using CMSTrain.Application.DTOs.Training;
using CMSTrain.Application.Common.Service;
using CMSTrain.Application.DTOs.ClassTrainers;

namespace CMSTrain.Application.Interfaces.Services;

public interface IClassTrainersService : ITransientService
{
    List<GetTrainersDto> GetAllActiveTrainers(int pageNumber, int pageSize, out int rowCount);

    List<GetTrainersDto> GetAllActiveTrainers();
    
    List<GetTrainingDto> GetAllAvailableTrainingsForTrainers(int pageNumber, int pageSize, out int rowCount, string? search = null);
    
    List<GetTrainingDto> GetAllAvailableTrainingsForTrainers(string? search = null);

    AvailableTrainingCountDto GetAllAvailableTrainingCountForTrainers();
    
    List<GetAssignedTrainingsDto> GetAllAssignedTrainingsForTrainers(int statusAction, int pageNumber, int pageSize, out int rowCount, string? search = null);

    List<GetAssignedTrainingsDto> GetAllAssignedTrainingsForTrainers(int statusAction, string? search = null);
    
    AssignedTrainingCountDto GetAllAssignedTrainingCountForTrainers();

    List<GetAssignedTrainersDto> GetAllTrainersForTraining(Guid trainingId, int pageNumber, int pageSize, out int rowCount, string? search = null);

    List<GetAssignedTrainersDto> GetAllTrainersForTraining(Guid trainingId, string? search = null);
    
    List<GetAssignedTrainersDto> GetAllTrainersForClass(Guid classId, string? search = null);

    List<GetAssignedTrainersDto> GetAllTrainersForClass(Guid classId, int pageNumber, int pageSize, out int rowCount, string? search = null);
    
    void AssignTrainersToClass(AssignTrainersDto trainingAssignment);
    
    void UpdateTrainerDescription(Guid classTrainerId, UpdateClassTrainerDescriptionDto classTrainerDescription);
    
    GetTrainerDescriptionDto GetTrainerDescriptionsOnTraining(Guid trainingId, Guid trainerId);

    GetTrainerDescriptionDto GetTrainerDescriptionsOnClass(Guid classId, Guid trainerId);
}
