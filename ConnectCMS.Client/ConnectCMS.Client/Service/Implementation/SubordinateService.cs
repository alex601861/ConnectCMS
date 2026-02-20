using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Models.Responses.Subordinate;
using CMSTrain.Client.Service.Base;
using CMSTrain.Client.Service.Interface;
using System.Text.Json;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Requests.Subordinate;

namespace CMSTrain.Client.Service.Implementation;

public class SubordinateService(IBaseService baseService) : ISubordinateService
{
    public async Task<ResponseDto<GetSubordinateDto?>?> GetSubordinateById(Guid subordinateId)
    {
        var pathParameter = new List<string>()
        {
            subordinateId.ToString()
        };
        
        var response = await baseService.GetAsync<GetSubordinateDto?>(ApiEndpoints.Subordinate.GetSubordinateById, pathParameter);

        return response;
    }

    public async Task<CollectionDto<GetSubordinateDto>?> GetSubordinateDetails(Guid trainingId, int pageNumber, int pageSize, string? search = null, int? type = null)
    {
        var pathParameter = new List<string>()
        {
            trainingId.ToString()
        };

        var parameter = new Dictionary<string, string?>()
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search },
            { "type", type.ToString() }
        };
        
        var response = await baseService.GetPagedAsync<GetSubordinateDto>(ApiEndpoints.Subordinate.GetSubordinateDetails, pathParameter, parameter);

        return response;
    }
    
    public async Task<ResponseDto<List<GetSubordinateDto>?>?> GetSubordinateDetails(Guid trainingId)
    {
        var pathParameter = new List<string>()
        {
            trainingId.ToString()
        };
        
        var response = await baseService.GetAsync<List<GetSubordinateDto>?>(ApiEndpoints.Subordinate.GetSubordinateDetailsList, pathParameter);

        return response;
    }

    public async Task<CollectionDto<GetSubordinateDto>?> GetSubordinateDetailsForTrainingCandidate(Guid trainingCandidateId, int pageNumber, int pageSize)
    {
        var pathParameter = new List<string>()
        {
            trainingCandidateId.ToString()
        };
        
        var parameter = new Dictionary<string, string?>()
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() }
        };
        
        var response = await baseService.GetPagedAsync<GetSubordinateDto>(ApiEndpoints.Subordinate.GetSubordinateDetailsForTrainingCandidate, pathParameter, parameter);

        return response;
    }
    
    public async Task<ResponseDto<List<GetSubordinateDto>?>?> GetSubordinateDetailsForTrainingCandidate(Guid trainingCandidateId)
    {
        var pathParameter = new List<string>()
        {
            trainingCandidateId.ToString()
        };
        
        var response = await baseService.GetAsync<List<GetSubordinateDto>?>(ApiEndpoints.Subordinate.GetSubordinateDetailsForTrainingCandidateList, pathParameter);

        return response;
    }
    
    public async Task<ResponseDto<GetSubordinateDto?>?> GetSubordinateDetails(Guid trainingId, SubordinateType subordinateType)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString(),
            subordinateType.ToString()
        };
        
        var response = await baseService.GetAsync<GetSubordinateDto?>(ApiEndpoints.Subordinate.GetSubordinateViewDetails, pathParameter);

        return response;
    }
    
    public async Task<ResponseDto<bool?>?> InsertSubordinateForCandidates(CreateCandidateSubordinateDto candidateSubordinate)
    {
        var jsonRequest = JsonSerializer.Serialize(candidateSubordinate);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

        var response = await baseService.PostAsync<bool?>(ApiEndpoints.Subordinate.InsertSubordinateForCandidates, content);

        return response;
    }
    
    public async Task<ResponseDto<bool?>?> InsertSubordinateForCandidates(CreateClientSubordinateDto subordinate)
    {
        var jsonRequest = JsonSerializer.Serialize(subordinate);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

        var response = await baseService.PostAsync<bool?>(ApiEndpoints.Subordinate.InsertSubordinateForTrainingCandidates, content);

        return response;
    }
}