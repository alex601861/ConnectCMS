using System.Globalization;
using System.Net.Http.Headers;
using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Base;
using CMSTrain.Application.DTOs.Count;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Interface;
using CMSTrain.Client.Models.Responses.Count;
using CMSTrain.Client.Models.Requests.Training;
using CMSTrain.Client.Models.Responses.Training;
using CMSTrain.Client.Models.Responses.ClassTrainers;
using CMSTrain.Client.Models.Responses.Organization;
using CMSTrain.Client.Models.Responses.TrainingCandidate;

namespace CMSTrain.Client.Service.Implementation;

public class TrainingService(IBaseService baseService) : ITrainingService
{
    #region Admin
    public async Task<CollectionDto<GetTrainingDto>?> GetAllTrainings(int statusAction, int pageNumber, int pageSize, string? search, bool? isActive)
    {
        var pathParameter = new List<string>
        {
            statusAction.ToString()
        };
        
        var queryParameter = new Dictionary<string, string?>()
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search },
            { "isActive", isActive?.ToString() }
        };

        var response = await baseService.GetPagedAsync<GetTrainingDto>(endpoint: ApiEndpoints.Training.GetAllTrainings, pathParameter, queryParameter);

        return response;
    }
    
    public async Task<ResponseDto<List<GetTrainingDto>?>?> GetAllTrainings(int statusAction, string? search, bool? isActive)
    {
        var pathParameter = new List<string>
        {
            statusAction.ToString()
        };
        
        var queryParameter = new Dictionary<string, string?>()
        {
            { "search", search },
            { "isActive", isActive?.ToString() }
        };
        
        var response = await baseService.GetAsync<List<GetTrainingDto>?>(ApiEndpoints.Training.GetAllTrainingsList, pathParameter, queryParameter);

        return response;
    }

    public async Task<ResponseDto<AssignedTrainingCountDto?>?> GetAvailableTrainingsCount()
    {
        var response = await baseService.GetAsync<AssignedTrainingCountDto>(ApiEndpoints.Training.GetAvailableTrainingsCount);

        return response;
    }
    
    public async Task<ResponseDto<GetTrainingDto?>?> GetTrainingById(Guid trainingId)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString()
        };
        
        var response = await baseService.GetAsync<GetTrainingDto?>(ApiEndpoints.Training.GetTrainingById, pathParameter);

        return response;
    }

    public async Task<ResponseDto<TrainingModuleCountDto?>?> GetAllTrainingModuleCount(bool? isActive = null)
    {
        var parameter = new Dictionary<string, string?>()
        {
            { "isActive", isActive.ToString() }
        };
        
        var response = await baseService.GetAsync<TrainingModuleCountDto?>(ApiEndpoints.Training.GetTrainingModuleCount, parameters: parameter);

        return response;
    }
    
    public async Task<ResponseDto<TrainingDetailsCountDto?>?> GetTrainingDetailsCount(Guid trainingId)
    {
        var pathParameter = new List<string>()
        {
            trainingId.ToString()
        };

        var response = await baseService.GetAsync<TrainingDetailsCountDto?>(ApiEndpoints.Training.GetTrainingDetailsCount, pathParameter);

        return response;
    }
    
    public async Task<ResponseDto<List<GetOrganizationDto>?>?> GetAllAssignedClientOrganizations(Guid trainingId)
    {
        var pathParameter = new List<string>()
        {
            trainingId.ToString()
        };

        var response = await baseService.GetAsync<List<GetOrganizationDto>?>(ApiEndpoints.Training.GetAllAssignedClientOrganizations, pathParameter);

        return response;
    }
    
    public async Task<ResponseDto<bool?>?> InsertTraining(CreateTrainingDto training)
    {
        var formData = new MultipartFormDataContent();

        formData.Add(new StringContent(training.Title), "Title");
        formData.Add(new StringContent(training.StartDate.ToString()!), "StartDate");
        formData.Add(new StringContent(training.EndDate.ToString()!), "EndDate");
        formData.Add(new StringContent(training.Description), "Description");
        formData.Add(new StringContent(training.LocationDetails), "LocationDetails");
        formData.Add(new StringContent(training.Longitude?.ToString(CultureInfo.InvariantCulture) ?? "0.00"), "Longitude");
        formData.Add(new StringContent(training.Latitude?.ToString(CultureInfo.InvariantCulture) ?? "0.00"), "Latitude");
        formData.Add(new StringContent(training.TrainingFormatId.ToString()), "TrainingFormatId");

        if (training.Image != null)
        {
            var organizationFileContent = new StreamContent(training.Image!.OpenReadStream(long.MaxValue));
            
            organizationFileContent.Headers.ContentType = new MediaTypeHeaderValue(training.Image.ContentType);
            
            formData.Add(organizationFileContent, "Image", training.Image.Name);
        }

        var response = await baseService.UploadAsync<bool?>(ApiEndpoints.Training.InsertTraining, Constants.UploadType.Post, formData);

        return response;
    }

    public async Task<ResponseDto<bool?>?> UpdateTraining(UpdateTrainingDto training)
    {
        var formData = new MultipartFormDataContent();

        formData.Add(new StringContent(training.Id.ToString()), "Id");
        formData.Add(new StringContent(training.Title), "Title");
        formData.Add(new StringContent(training.StartDate.ToString() ?? DateTime.Now.AddDays(1).ToString("dd-MM-yyyy")), "StartDate");
        formData.Add(new StringContent(training.EndDate.ToString() ?? DateTime.Now.AddDays(2).ToString("dd-MM-yyyy")), "EndDate");
        formData.Add(new StringContent(training.Description), "Description");
        formData.Add(new StringContent(training.LocationDetails), "LocationDetails");
        formData.Add(new StringContent(training.Longitude?.ToString(CultureInfo.InvariantCulture)  ?? "0.00"), "Longitude");
        formData.Add(new StringContent(training.Latitude?.ToString(CultureInfo.InvariantCulture)  ?? "0.00"), "Latitude");
        formData.Add(new StringContent(training.TrainingFormatId.ToString()), "TrainingFormatId");

        if (training.Image != null)
        {
            var organizationFileContent = new StreamContent(training.Image!.OpenReadStream(long.MaxValue));
            
            organizationFileContent.Headers.ContentType = new MediaTypeHeaderValue(training.Image.ContentType);
            
            formData.Add(organizationFileContent, "Image", training.Image.Name);
        }

        var response = await baseService.UploadAsync<bool?>(ApiEndpoints.Training.UpdateTraining, Constants.UploadType.Put, formData);

        return response;
    }

    public async Task<ResponseDto<bool?>?> ActivateDeactivateTraining(Guid trainingId)
    {
        var pathParameter = new List<string>()
        {
            trainingId.ToString()
        };

        var response = await baseService.DeleteAsync<bool?>(ApiEndpoints.Training.ActivateDeactivateTraining, Constants.DeleteType.Patch, pathParameter);
        
        return response;
    }
    #endregion

    #region Trainer
    public async Task<CollectionDto<GetTrainingDto>?> GetAllTrainingsForTrainer(int pageNumber, int pageSize, string? search = null)
    {
        var queryParameter = new Dictionary<string, string?>()
        {

            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search },
        };
        
        var response = await baseService.GetPagedAsync<GetTrainingDto>(ApiEndpoints.Training.GetAllTrainingsForTrainer, parameters: queryParameter);

        return response;
    }
    
    public async Task<ResponseDto<List<GetTrainingDto>?>?> GetAllTrainingsForTrainer()
    {
        var response = await baseService.GetAsync<List<GetTrainingDto>?>(ApiEndpoints.Training.GetAllTrainingsForTrainerList);

        return response;
    }

    public async Task<ResponseDto<AvailableTrainingCountDto?>?> GetAllAvailableTrainingCountForTrainers()
    {
        var response = await baseService.GetAsync<AvailableTrainingCountDto>(ApiEndpoints.Training.GetAvailableTrainingCountForTrainers);

        return response;
    }
    
    public async Task<CollectionDto<GetAssignedTrainingsDto>?> GetAllAssignedTrainingsForTrainers(int requestAction, int pageNumber, int pageSize, string? search = null)
    {
        var pathParameter = new List<string>()
        {
            requestAction.ToString()
        };
        
        var queryParameter = new Dictionary<string, string?>()
        {

            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search },
        };
        
        var response = await baseService.GetPagedAsync<GetAssignedTrainingsDto>(ApiEndpoints.Training.GetAssignedTrainingsForTrainer, pathParameter, queryParameter);

        return response;
    }
    
    public async Task<ResponseDto<List<GetAssignedTrainingsDto>?>?> GetAllAssignedTrainingsForTrainers(int requestAction)
    {
        var pathParameter = new List<string>()
        {
            requestAction.ToString()
        };
        
        var response = await baseService.GetAsync<List<GetAssignedTrainingsDto>?>(ApiEndpoints.Training.GetAssignedTrainingsForTrainerList, pathParameter);

        return response;
    }
    
    public async Task<ResponseDto<AssignedTrainingCountDto?>?> GetAllAssignedTrainingCountForTrainers()
    {
        var response = await baseService.GetAsync<AssignedTrainingCountDto>(ApiEndpoints.Training.GetAllAssignedTrainingCountForTrainers);

        return response;
    }
    #endregion

    #region Client
    public async Task<CollectionDto<GetAllTrainingsForClient>?> GetAllTrainingsForClient(int requestAction, int pageNumber, int pageSize, string? search = null)
    {
        var pathParameter = new List<string>()
        {
            requestAction.ToString()
        };
        
        var queryParameter = new Dictionary<string, string?>()
        {

            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search },
        };
        
        var response = await baseService.GetPagedAsync<GetAllTrainingsForClient>(ApiEndpoints.Training.GetAllTrainingsForClient, pathParameter, queryParameter);

        return response;
    }

    public async Task<ResponseDto<List<GetAllTrainingsForClient>?>?> GetAllTrainingsForClient(int requestAction)
    {
        var pathParameter = new List<string>()
        {
            requestAction.ToString()
        };
        
        var response = await baseService.GetAsync<List<GetAllTrainingsForClient>?>(ApiEndpoints.Training.GetAllTrainingsForClientList, pathParameter);

        return response;
    }

    public async Task<ResponseDto<AvailableTrainingCountDto?>?> GetAllAvailableTrainingCountsForClient()
    {
        var response = await baseService.GetAsync<AvailableTrainingCountDto>(ApiEndpoints.Training.GetAvailableTrainingCountForClient);

        return response;
    }
    
    public async Task<CollectionDto<GetAllTrainingsForClient>?> GetAllAssignedTrainingsForClient(int requestAction, int pageNumber, int pageSize, string? search = null)
    {
        var pathParameter = new List<string>()
        {
            requestAction.ToString()
        };
        
        var queryParameter = new Dictionary<string, string?>()
        {

            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search },
        };
        
        var response = await baseService.GetPagedAsync<GetAllTrainingsForClient>(ApiEndpoints.Training.GetAssignedTrainingsForClient, pathParameter, queryParameter);

        return response;
    }

    public async Task<ResponseDto<List<GetAllTrainingsForClient>?>?> GetAllAssignedTrainingsForClient(int requestAction)
    {
        var pathParameter = new List<string>()
        {
            requestAction.ToString()
        };
        
        var response = await baseService.GetAsync<List<GetAllTrainingsForClient>?>(ApiEndpoints.Training.GetAssignedTrainingsForClientList, pathParameter);

        return response;
    }
    
    public async Task<ResponseDto<AssignedTrainingCountDto?>?> GetAllAssignedTrainingCountsForClient()
    {
        var response = await baseService.GetAsync<AssignedTrainingCountDto>(ApiEndpoints.Training.GetAllAssignedTrainingCountsForClient);

        return response;
    }
    
    public async Task<ResponseDto<TrainingDetailsCountDto?>?> GetTrainingDetailsCountForClient(Guid trainingId)
    {
        var pathParameter = new List<string>()
        {
            trainingId.ToString()
        };
        
        var response = await baseService.GetAsync<TrainingDetailsCountDto>(ApiEndpoints.Training.GetTrainingDetailsCountForClient, pathParameter);

        return response;
    }
    #endregion

    #region Candidates
    public async Task<CollectionDto<GetAllTrainingsForCandidate>?> GetAllTrainingsForCandidate(int requestAction, int pageNumber, int pageSize, string? search = null)
    {
        var pathParameter = new List<string>()
        {
            requestAction.ToString()
        };
        
        var queryParameter = new Dictionary<string, string?>()
        {

            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search },
        };
        
        var response = await baseService.GetPagedAsync<GetAllTrainingsForCandidate>(ApiEndpoints.Training.GetAllTrainingsForCandidate, pathParameter, queryParameter);

        return response;
    }

    public async Task<ResponseDto<List<GetAllTrainingsForCandidate>?>?> GetAllTrainingsForCandidate(int requestAction)
    {
        var pathParameter = new List<string>()
        {
            requestAction.ToString()
        };
        
        var response = await baseService.GetAsync<List<GetAllTrainingsForCandidate>?>(ApiEndpoints.Training.GetAllTrainingsForCandidateList, pathParameter);

        return response;
    }

    public async Task<ResponseDto<AvailableTrainingCountDto?>?> GetAvailableTrainingCountsForCandidate()
    {
        var response = await baseService.GetAsync<AvailableTrainingCountDto?>(ApiEndpoints.Training.GetAvailableTrainingCountsForCandidate);

        return response;
    }
    
    public async Task<CollectionDto<GetAllTrainingsForCandidate>?> GetAllAssignedTrainingsForCandidate(int requestAction, int pageNumber, int pageSize, string? search = null)
    {
        var pathParameter = new List<string>()
        {
            requestAction.ToString()
        };
        
        var queryParameter = new Dictionary<string, string?>()
        {

            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search },
        };
        
        var response = await baseService.GetPagedAsync<GetAllTrainingsForCandidate>(ApiEndpoints.Training.GetAssignedTrainingsForCandidate, pathParameter, queryParameter);

        return response;
    }

    public async Task<ResponseDto<List<GetAllTrainingsForCandidate>?>?> GetAllAssignedTrainingsForCandidate(int requestAction)
    {
        var pathParameter = new List<string>()
        {
            requestAction.ToString()
        };
        
        var response = await baseService.GetAsync<List<GetAllTrainingsForCandidate>?>(ApiEndpoints.Training.GetAssignedTrainingsForCandidateList, pathParameter);

        return response;
    }
    
    public async Task<ResponseDto<AssignedTrainingCountDto?>?> GetAssignedTrainingCountsForCandidate()
    {
        var response = await baseService.GetAsync<AssignedTrainingCountDto>(ApiEndpoints.Training.GetAllAssignedTrainingCountsForCandidate);

        return response;
    }

    public async Task<ResponseDto<TrainingDetailsCountDto?>?> GetTrainingDetailsCountForCandidate(Guid trainingId)
    {
        var pathParameter = new List<string>()
        {
            trainingId.ToString()
        };
        
        var response = await baseService.GetAsync<TrainingDetailsCountDto?>(ApiEndpoints.Training.GetTrainingDetailsCountForCandidate, pathParameter);

        return response;
    }
    #endregion

    #region Generic
    public async Task<ResponseDto<GetTrainingDto?>?> GetTrainingDetailsByInspection(Guid trainingInspectionId)
    {
        var pathParameter = new List<string>()
        {
            trainingInspectionId.ToString()
        };
        
        var response = await baseService.GetAsync<GetTrainingDto>(ApiEndpoints.Training.GetTrainingDetailsByInspection, pathParameter);

        return response;
    }
    
    public async Task<ResponseDto<GetTrainingDto?>?> GetTrainingDetailsByQuestionnaire(Guid questionnaireId)
    {
        var pathParameter = new List<string>()
        {
            questionnaireId.ToString()
        };
        
        var response = await baseService.GetAsync<GetTrainingDto?>(ApiEndpoints.Training.GetTrainingDetailsByQuestionnaire, pathParameter);

        return response;
    }
    #endregion
}