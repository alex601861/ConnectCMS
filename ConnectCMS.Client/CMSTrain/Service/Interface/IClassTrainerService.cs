using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Dependency;
using CMSTrain.Client.Models.Responses.Training;
using CMSTrain.Client.Models.Requests.ClassTrainers;
using CMSTrain.Client.Models.Responses.ClassTrainers;

namespace CMSTrain.Client.Service.Interface;

public interface IClassTrainerService : ITransientService
{
    Task<CollectionDto<GetTrainingDto>?> GetAllAvailableTrainingsForTrainers(int pageNumber, int pageSize);

    Task<ResponseDto<List<GetTrainingDto>?>?> GetAllAvailableTrainingsForTrainers();

    Task<CollectionDto<GetAssignedTrainersDto>?> GetAllTrainersForTraining(Guid trainingId, int pageNumber, int pageSize, string? search = null);
    
    Task<ResponseDto<List<GetAssignedTrainersDto>?>?> GetAllTrainersForTraining(Guid trainingId, string? search = null);

    Task<CollectionDto<GetAssignedTrainersDto>?> GetAllTrainersForClass(Guid trainingId, int pageNumber, int pageSize, string? search = null);

    Task<ResponseDto<List<GetAssignedTrainersDto>?>?> GetAllTrainersForClass(Guid classId, string? search = null);

    Task<CollectionDto<GetAssignedTrainingsDto>?> GetAllAssignedTrainingsForTrainers(int pageNumber, int pageSize);

    Task<ResponseDto<List<GetAssignedTrainingsDto>?>?> GetAllAssignedTrainingsForTrainers();

    Task<ResponseDto<bool?>?> AssignTrainersToClass(AssignTrainersDto assignTrainersDto);

    Task<ResponseDto<bool?>?> UpdateTrainerDescription(UpdateClassTrainerDescriptionDto classTrainerDescription);

    Task<ResponseDto<GetTrainerDescriptionDto?>?> GetTrainerDescriptionsOnTraining(Guid trainingId, Guid trainerId);

    Task<ResponseDto<GetTrainerDescriptionDto?>?> GetTrainerDescriptionsOnClass(Guid classId, Guid trainerId);
}