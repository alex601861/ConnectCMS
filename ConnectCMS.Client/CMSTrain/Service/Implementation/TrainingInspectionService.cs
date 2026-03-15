using System.Text.Json;
using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Requests.TrainingInspection;
using CMSTrain.Client.Models.Responses.TrainingInspection;
using CMSTrain.Client.Service.Base;
using CMSTrain.Client.Service.Interface;

namespace CMSTrain.Client.Service.Implementation;

public class TrainingInspectionService(IBaseService baseService) : ITrainingInspectionService
{
    public async Task<ResponseDto<GetTrainingInspectionDetailsDto?>?> GetTrainingInspectionById(Guid trainingInspectionId)
    {
        var pathParameter = new List<string>
        {
            trainingInspectionId.ToString(),
        };
        
        var response = await baseService.GetAsync<GetTrainingInspectionDetailsDto?>(ApiEndpoints.TrainingInspection.GetTrainingInspectionById, pathParameter);

        return response;
    }

    public async Task<ResponseDto<GetTrainingInspectionDetailsDto?>?> GetTrainingInspectionByQuestionnaire(Guid questionnaireId)
    {
        var pathParameter = new List<string>
        {
            questionnaireId.ToString(),
        };
        
        var response = await baseService.GetAsync<GetTrainingInspectionDetailsDto?>(ApiEndpoints.TrainingInspection.GetTrainingInspectionByQuestionnaire, pathParameter);

        return response;
    }
    
    public async Task<CollectionDto<GetTrainingInspectionDto>?> GetAllAssignedTrainingInspections(Guid trainingId, int pageNumber, int pageSize, string? search)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString()
        };
        
        var parameter = new Dictionary<string, string?>
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search }
        };
        
        var response = await baseService.GetPagedAsync<GetTrainingInspectionDto>(ApiEndpoints.TrainingInspection.GetAllTrainingInspections, pathParameter, parameter);

        return response;
    }
    
    public async Task<ResponseDto<List<GetTrainingInspectionDto>?>?> GetAllAssignedTrainingInspections(Guid trainingId, string? search)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString()
        };
        
        var parameter = new Dictionary<string, string?>
        {
            { "search", search },
        };
        
        var response = await baseService.GetAsync<List<GetTrainingInspectionDto>?>(ApiEndpoints.TrainingInspection.GetAllTrainingInspectionsList, pathParameter, parameter);

        return response;
    }

    public async Task<CollectionDto<GetTrainingInspectionDto>?> GetAllAssignedTrainingInspectionsForCandidate(Guid trainingId, int pageNumber, int pageSize, string? search)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString()
        };
        
        var parameter = new Dictionary<string, string?>
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search }
        };
        
        var response = await baseService.GetPagedAsync<GetTrainingInspectionDto>(ApiEndpoints.TrainingInspection.GetAllTrainingInspectionsForCandidate, pathParameter, parameter);

        return response;
    }

    public async Task<ResponseDto<List<GetTrainingInspectionDto>?>?> GetAllAssignedTrainingInspectionsForCandidate(Guid trainingId, string? search)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString()
        };
        
        var parameter = new Dictionary<string, string?>
        {
            { "search", search },
        };
        
        var response = await baseService.GetAsync<List<GetTrainingInspectionDto>?>(ApiEndpoints.TrainingInspection.GetAllTrainingInspectionsForCandidateList, pathParameter, parameter);

        return response;
    }

    public async Task<CollectionDto<GetTrainingInspectionDto>?> GetAllAssignedTrainingInspectionsForClient(Guid trainingId, int pageNumber, int pageSize, string? search)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString()
        };
        
        var parameter = new Dictionary<string, string?>
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search }
        };
        
        var response = await baseService.GetPagedAsync<GetTrainingInspectionDto>(ApiEndpoints.TrainingInspection.GetAllTrainingInspectionsForClient, pathParameter, parameter);

        return response;
    }

    public async Task<ResponseDto<List<GetTrainingInspectionDto>?>?> GetAllAssignedTrainingInspectionsForClient(Guid trainingId, string? search)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString()
        };
        
        var parameter = new Dictionary<string, string?>
        {
            { "search", search },
        };
        
        var response = await baseService.GetAsync<List<GetTrainingInspectionDto>?>(ApiEndpoints.TrainingInspection.GetAllTrainingInspectionsForClientList, pathParameter, parameter);

        return response;
    }

    
    public async Task<CollectionDto<GetTrainingInspectionDto>?> GetAllAssignedTrainingInspectionsForTrainingCandidate(Guid trainingCandidateId, int pageNumber, int pageSize, string? search)
    {
        var pathParameter = new List<string>
        {
            trainingCandidateId.ToString()
        };
        
        var parameter = new Dictionary<string, string?>
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search }
        };
        
        var response = await baseService.GetPagedAsync<GetTrainingInspectionDto>(ApiEndpoints.TrainingInspection.GetAllTrainingInspectionsForTrainingCandidate, pathParameter, parameter);

        return response;
    }

    public async Task<ResponseDto<List<GetTrainingInspectionDto>?>?> GetAllAssignedTrainingInspectionsForTrainingCandidate(Guid trainingCandidateId, string? search)
    {
        var pathParameter = new List<string>
        {
            trainingCandidateId.ToString()
        };
        
        var parameter = new Dictionary<string, string?>
        {
            { "search", search },
        };
        
        var response = await baseService.GetAsync<List<GetTrainingInspectionDto>?>(ApiEndpoints.TrainingInspection.GetAllTrainingInspectionsForTrainingCandidateList, pathParameter, parameter);

        return response;
    }
    
    public async Task<ResponseDto<GetCandidateTrainingInspectionDto?>?> GetCandidateTrainingInspectionDetails(Guid trainingInspectionId)
    {
        var pathParameter = new List<string>
        {
            trainingInspectionId.ToString()
        };
        
        var response = await baseService.GetAsync<GetCandidateTrainingInspectionDto?>(ApiEndpoints.TrainingInspection.GetCandidateTrainingInspectionDetails, pathParameter);

        return response;
    }

    public async Task<ResponseDto<GetSubordinateTrainingInspectionDto?>?> GetSubordinateTrainingInspectionDetails(Guid subordinateId)
    {
        var pathParameter = new List<string>
        {
            subordinateId.ToString()
        };
        
        var response = await baseService.GetAsync<GetSubordinateTrainingInspectionDto?>(ApiEndpoints.TrainingInspection.GetSubordinateTrainingInspectionDetails, pathParameter);

        return response;
    }
    
    public async Task<ResponseDto<GetCandidateTrainingInspectionDto?>?> GetCandidateTrainingInspectionDetailsForTrainingCandidate(Guid trainingCandidateId, Guid trainingInspectionId)
    {
        var pathParameter = new List<string>
        {
            trainingCandidateId.ToString(),
            trainingInspectionId.ToString()
        };
        
        var response = await baseService.GetAsync<GetCandidateTrainingInspectionDto?>(ApiEndpoints.TrainingInspection.GetCandidateTrainingInspectionDetailsForTrainingCandidate, pathParameter);

        return response;
    }
    
    public async Task<ResponseDto<GetTrainingInspectionQuestionnaireCountDto?>?> GetTrainingInspectionQuestionnairesCount(Guid trainingId)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString()
        };
        
        var response = await baseService.GetAsync<GetTrainingInspectionQuestionnaireCountDto?>(ApiEndpoints.TrainingInspection.GetTrainingInspectionQuestionnairesCount, pathParameter);

        return response;
    }

    public async Task<ResponseDto<int?>?> GetTrainingInspectionPhaseCounts(Guid trainingInspectionId)
    {
        var pathParameter = new List<string>
        {
            trainingInspectionId.ToString()
        };
        
        var response = await baseService.GetAsync<int?>(ApiEndpoints.TrainingInspection.GetTrainingInspectionPhaseCounts, pathParameter);

        return response;
    }
    
    public async Task<ResponseDto<bool?>?> AssignTrainingInspections(AssignTrainingInspectionDto trainingInspections)
    {
        var jsonRequest = JsonSerializer.Serialize(trainingInspections);
        
        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

        var response = await baseService.PostAsync<bool?>(ApiEndpoints.TrainingInspection.AssignTrainingInspections, content);

        return response;
    }
}