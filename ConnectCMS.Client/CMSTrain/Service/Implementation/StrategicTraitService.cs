using System.Text.Json;
using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Interface;
using CMSTrain.Client.Models.Requests.Strategy;
using CMSTrain.Client.Models.Responses.Strategy;

namespace CMSTrain.Client.Service.Implementation;

public class StrategicTraitService(IBaseService baseService) : IStrategicTraitService
{
    public async Task<CollectionDto<GetStrategyDto>?> GetAllStrategies(StrategicType traitType, int pageNumber, int pageSize, string? search = null)
    {
        var pathParameter = new List<string>
        {
            traitType.ToString()
        };
        
        var queryParameter = new Dictionary<string, string?>()
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search }
        };

        var response = await baseService.GetPagedAsync<GetStrategyDto>(endpoint: ApiEndpoints.StrategicTrait.GetAllStrategies, pathParameter, queryParameter);

        return response;
    }

    public async Task<ResponseDto<List<GetStrategyDto>?>?> GetAllStrategies()
    {
        var response = await baseService.GetAsync<List<GetStrategyDto>?>(ApiEndpoints.StrategicTrait.GetAllStrategiesList);

        return response;
    }

    public async Task<ResponseDto<List<GetStrategyModuleDto>?>?> GetAllStrategyModules(StrategicType type)
    {
        var pathParameter = new List<string>
        {
            type.ToString()
        };
        
        var response = await baseService.GetAsync<List<GetStrategyModuleDto>?>(ApiEndpoints.StrategicTrait.GetAllStrategyModules, pathParameter);

        return response;
    }

    public async Task<ResponseDto<GetAllStrategyTraitResultsDto?>?> GetAllStrategyTraitResults(string strengthIds, string weaknessIds)
    {
        var queryParameter = new Dictionary<string, string?>()
        {
            { "strengthIds", strengthIds },
            { "weaknessIds", weaknessIds }
        };

        var response = await baseService.GetAsync<GetAllStrategyTraitResultsDto>(endpoint: ApiEndpoints.StrategicTrait.GetAllStrategyTraitResults, parameters: queryParameter);

        return response;
    }
    
    public async Task<ResponseDto<GetStrategyDto?>?> GetStrategyById(Guid strategyId)
    {
        var pathParameter = new List<string>
        {
            strategyId.ToString()
        };
        
        var response = await baseService.GetAsync<GetStrategyDto?>(ApiEndpoints.StrategicTrait.GetStrategyById, pathParameter);

        return response;
    }

    public async Task<ResponseDto<GetStrategyDetailsDto?>?> GetStrategyDetails()
    {
        var response = await baseService.GetAsync<GetStrategyDetailsDto?>(ApiEndpoints.StrategicTrait.GetStrategyDetails);

        return response;
    }

    public async Task<ResponseDto<bool?>?> InsertStrategy(InsertStrategyDto strategy)
    {
        var jsonRequest = JsonSerializer.Serialize(strategy);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.PostAsync<bool?>(ApiEndpoints.StrategicTrait.InsertStrategy, content);
        
        return response;
    }

    public async Task<ResponseDto<bool?>?> UpdateStrategy(UpdateStrategyDto strategy)
    {
        var jsonRequest = JsonSerializer.Serialize(strategy);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.UpdateAsync<bool?>(ApiEndpoints.StrategicTrait.UpdateStrategy, Constants.UpdateType.Put, content);
        
        return response;
    }

    public async Task<ResponseDto<bool?>?> DeleteStrategy(Guid strategyId)
    {
        var pathParameter = new List<string>
        {
            strategyId.ToString()
        };
        
        var response = await baseService.DeleteAsync<bool?>(ApiEndpoints.StrategicTrait.DeleteStrategy, Constants.DeleteType.Delete, pathParameter);

        return response;
    }

    public async Task<ResponseDto<bool?>?> UploadStrategyDetails(UploadStrategyDetailsDto strategyDetails)
    {
        var jsonRequest = JsonSerializer.Serialize(strategyDetails);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.PostAsync<bool?>(ApiEndpoints.StrategicTrait.UploadStrategyDetails, content);
        
        return response;
    }

    public async Task<ResponseDto<bool?>?> UploadStrategyTraitQuestionnaire(UploadStrategyTraitQuestionnaireDto strategyDetails)
    {
        var jsonRequest = JsonSerializer.Serialize(strategyDetails);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.PostAsync<bool?>(ApiEndpoints.StrategicTrait.UploadStrategyTraitQuestionnaire, content);
        
        return response;
    }

    public async Task<ResponseDto<GetStrategicTraitCountDto?>?> GetStrategicTraitCount()
    {
        var response = await baseService.GetAsync<GetStrategicTraitCountDto?>(ApiEndpoints.StrategicTrait.GetStrategicTraitCount);

        return response;
    }
    
    public async Task<CollectionDto<GetStrategyTraitQuestionnaireDto>?> GetStrategyTraitQuestionnaireResponses(int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null)
    {
        var queryParameter = new Dictionary<string, string?>()
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "startDate", startDate?.ToString("yyyy-MM-dd") },
            { "endDate", endDate?.ToString("yyyy-MM-dd") }
        };

        var response = await baseService.GetPagedAsync<GetStrategyTraitQuestionnaireDto>(endpoint: ApiEndpoints.StrategicTrait.GetStrategyTraitQuestionnaireResponses, parameters: queryParameter);

        return response;
    }
    
    public async Task<ResponseDto<List<GetStrategyTraitQuestionnaireDto>?>?> GetStrategyTraitQuestionnaireResponses( DateTime? startDate = null, DateTime? endDate = null)
    {
        var queryParameter = new Dictionary<string, string?>()
        {
            { "startDate", startDate?.ToString("yyyy-MM-dd") },
            { "endDate", endDate?.ToString("yyyy-MM-dd") }
        };
        
        var response = await baseService.GetAsync<List<GetStrategyTraitQuestionnaireDto>?>(ApiEndpoints.StrategicTrait.GetStrategyTraitQuestionnaireResponsesList, parameters: queryParameter);

        return response;
    }

    public async Task<CollectionDto<GetStrategyTraitQuestionnaireDto>?> GetStrategyTraitQuestionnaireResponsesByUserId(Guid userId, int pageNumber, int pageSize)
    {
        var pathParameter = new List<string>()
        {
            userId.ToString()
        };
        
        var queryParameter = new Dictionary<string, string?>()
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() }
        };

        var response = await baseService.GetPagedAsync<GetStrategyTraitQuestionnaireDto>(endpoint: ApiEndpoints.StrategicTrait.GetStrategyTraitQuestionnaireResponsesByUserId, pathParameter, queryParameter);

        return response;
    }

    public async Task<ResponseDto<List<GetStrategyTraitQuestionnaireDto>?>?> GetStrategyTraitQuestionnaireResponsesByUserId(Guid userId)
    {
        var pathParameter = new List<string>()
        {
            userId.ToString()
        };
        
        var response = await baseService.GetAsync<List<GetStrategyTraitQuestionnaireDto>?>(ApiEndpoints.StrategicTrait.GetStrategyTraitQuestionnaireResponsesByUserIdList, pathParameter);

        return response;
    }
    public async Task<ResponseDto<GetStrategyTraitQuestionnaireDetailsDto?>?> GetStrategyTraitQuestionnaireDetails(Guid responseId)
    {
        var pathParameter = new List<string>
        {
            responseId.ToString()
        };
        
        var response = await baseService.GetAsync<GetStrategyTraitQuestionnaireDetailsDto?>(ApiEndpoints.StrategicTrait.GetStrategyTraitQuestionnaireDetails, pathParameter);

        return response;
    }
}