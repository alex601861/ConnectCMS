using System.Text;
using System.Text.Json;
using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Interface;
using CMSTrain.Client.Models.Responses.Training;
using CMSTrain.Client.Models.Requests.ClassTrainers;
using CMSTrain.Client.Models.Responses.ClassTrainers;

namespace CMSTrain.Client.Service.Implementation;

public class ClassTrainerService(IBaseService baseService) : IClassTrainerService
{
    public async Task<CollectionDto<GetTrainingDto>?> GetAllAvailableTrainingsForTrainers(int pageNumber, int pageSize)
    {
        var queryParameter = new Dictionary<string, string?>()
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() }
        };
        
        var response = await baseService.GetPagedAsync<GetTrainingDto>(endpoint: ApiEndpoints.ClassTrainers.GetAllAvailableTrainingsForTrainers, parameters: queryParameter);

        return response;
    }
    
    public async Task<ResponseDto<List<GetTrainingDto>?>?> GetAllAvailableTrainingsForTrainers()
    {
        var response = await baseService.GetAsync<List<GetTrainingDto>?>(ApiEndpoints.ClassTrainers.GetAllAvailableTrainingsForTrainersList);

        return response;
    }

    public async Task<CollectionDto<GetAssignedTrainersDto>?> GetAllTrainersForTraining(Guid trainingId, int pageNumber, int pageSize, string? search = null)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString()
        };
        
        var queryParameter = new Dictionary<string, string?>()
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search }
        };
        
        var response = await baseService.GetPagedAsync<GetAssignedTrainersDto>(ApiEndpoints.ClassTrainers.GetAllTrainersForTraining, pathParameter, queryParameter);

        return response;
    }
    
    public async Task<ResponseDto<List<GetAssignedTrainersDto>?>?> GetAllTrainersForTraining(Guid trainingId, string? search = null)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString()
        };
        
        var queryParameter = new Dictionary<string, string?>()
        {
            { "search", search }
        };
        
        var response = await baseService.GetAsync<List<GetAssignedTrainersDto>?>(ApiEndpoints.ClassTrainers.GetAllTrainersForTrainingList, pathParameter, queryParameter);

        return response;
    }

    public async Task<CollectionDto<GetAssignedTrainersDto>?> GetAllTrainersForClass(Guid classId, int pageNumber, int pageSize, string? search = null)
    {
        var pathParameter = new List<string>
        {
            classId.ToString()
        };
        
        var queryParameter = new Dictionary<string, string?>()
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search }
        };
        
        var response = await baseService.GetPagedAsync<GetAssignedTrainersDto>(ApiEndpoints.ClassTrainers.GetAllTrainersForClass, pathParameter, queryParameter);

        return response;
    }
    
    public async Task<ResponseDto<List<GetAssignedTrainersDto>?>?> GetAllTrainersForClass(Guid classId, string? search = null)
    {
        var pathParameter = new List<string>
        {
            classId.ToString()
        };
        
        var queryParameter = new Dictionary<string, string?>()
        {
            { "search", search }
        };
        
        var response = await baseService.GetAsync<List<GetAssignedTrainersDto>?>(ApiEndpoints.ClassTrainers.GetAllTrainersForClassList, pathParameter, queryParameter);

        return response;
    }

    public async Task<CollectionDto<GetAssignedTrainingsDto>?> GetAllAssignedTrainingsForTrainers(int pageNumber, int pageSize)
    {
        var queryParameter = new Dictionary<string, string?>()
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() }
        };
        
        var response = await baseService.GetPagedAsync<GetAssignedTrainingsDto>(endpoint: ApiEndpoints.ClassTrainers.GetAllAssignedTrainingsForTrainers, parameters: queryParameter);

        return response;
    }
    
    public async Task<ResponseDto<List<GetAssignedTrainingsDto>?>?> GetAllAssignedTrainingsForTrainers()
    {
        var response = await baseService.GetAsync<List<GetAssignedTrainingsDto>?>(ApiEndpoints.ClassTrainers.GetAllAssignedTrainingsForTrainersList);

        return response;
    }
    
    public async Task<ResponseDto<bool?>?> AssignTrainersToClass(AssignTrainersDto assignTrainersDto)
    {
        var jsonRequest = JsonSerializer.Serialize(assignTrainersDto);

        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        
        var response = await baseService.PostAsync<bool?>(ApiEndpoints.ClassTrainers.AssignTrainersToClass, content);
        
        return response;
    }

    public async Task<ResponseDto<bool?>?> UpdateTrainerDescription( UpdateClassTrainerDescriptionDto classTrainerDescription)
    {
        var pathParameter = new List<string>
        {
            classTrainerDescription.ClassTrainerId.ToString()
        };

        var jsonRequest = JsonSerializer.Serialize(classTrainerDescription);
        
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        
        var response = await baseService.UpdateAsync<bool?>(ApiEndpoints.ClassTrainers.UpdateTrainerDescription, Constants.UpdateType.Put, content, pathParameter);

        return response;
    }

    public async Task<ResponseDto<GetTrainerDescriptionDto?>?> GetTrainerDescriptionsOnTraining(Guid trainingId, Guid trainerId)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString(),
            trainerId.ToString()
        };
        
        var response = await baseService.GetAsync<GetTrainerDescriptionDto>(ApiEndpoints.ClassTrainers.GetTrainerDescriptionsOnTraining, pathParameter);

        return response;
    }

    public async Task<ResponseDto<GetTrainerDescriptionDto?>?> GetTrainerDescriptionsOnClass(Guid classId, Guid trainerId)
    {
        var pathParameter = new List<string>
        {
            classId.ToString(),
            trainerId.ToString()
        };
        
        var response = await baseService.GetAsync<GetTrainerDescriptionDto>(ApiEndpoints.ClassTrainers.GetTrainerDescriptionsOnClass, pathParameter);

        return response;
    }
}