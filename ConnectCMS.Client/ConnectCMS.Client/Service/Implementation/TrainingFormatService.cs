using System.Text.Json;
using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Interface;
using CMSTrain.Client.Models.Requests.TrainingFormat;
using CMSTrain.Client.Models.Responses.TrainingFormat;
using CMSTrain.Client.Service.Base;
using UpdateType = CMSTrain.Client.Models.Constants.Constants.UpdateType;
using DeleteType = CMSTrain.Client.Models.Constants.Constants.DeleteType;

namespace CMSTrain.Client.Service.Implementation;

public class TrainingFormatService(IBaseService baseService) : ITrainingFormatService
{
    public async Task<CollectionDto<GetTrainingFormatDto>?> GetTrainingFormats(int pageNumber, int pageSize, string? search = null, bool? isActive = null)
    {
        var queryParameter = new Dictionary<string, string?>()
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search },
            { "isActive", isActive.ToString() }
        };

        var response = await baseService.GetPagedAsync<GetTrainingFormatDto>(endpoint: ApiEndpoints.TrainingFormat.GetAllTrainingFormats,parameters: queryParameter);

        return response;
    }
    
    public async Task<ResponseDto<List<GetTrainingFormatDto>?>?> GetTrainingFormats(string? search = null, bool? isActive = null)
    {
        var queryParameter = new Dictionary<string, string?>()
        {
            { "search", search },
            { "isActive", isActive.ToString() }
        };

        var response = await baseService.GetAsync<List<GetTrainingFormatDto>?>(ApiEndpoints.TrainingFormat.GetAllTrainingFormatsList, parameters: queryParameter);

        return response;
    }

    public async Task<ResponseDto<GetTrainingFormatDto?>?> GetTrainingFormatById(Guid trainingFormatId)
    {
        var pathParameter = new List<string>()
        {
            trainingFormatId.ToString()
        };
        
        var response = await baseService.GetAsync<GetTrainingFormatDto?>(ApiEndpoints.TrainingFormat.GetTrainingFormatById, pathParameter);

        return response;
    }

    public async Task<ResponseDto<bool?>?> InsertTrainingFormat(CreateTrainingFormatDto trainingFormat)
    {
        var jsonRequest = JsonSerializer.Serialize(trainingFormat);
        
        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

        var response = await baseService.PostAsync<bool?>(ApiEndpoints.TrainingFormat.InsertTrainingFormat, content);

        return response;
    }

    public async Task<ResponseDto<bool?>?> UpdateTrainingFormat(UpdateTrainingFormatDto trainingFormat)
    {
        var jsonRequest = JsonSerializer.Serialize(trainingFormat);
        
        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

        var response = await baseService.UpdateAsync<bool?>(ApiEndpoints.TrainingFormat.UpdateTrainingFormat, UpdateType.Put, content);

        return response;
    }

    public async Task<ResponseDto<bool?>?> DeleteTrainingFormat(Guid trainingFormatId)
    {
        var pathParameter = new List<string>()
        {
            trainingFormatId.ToString()
        };

        var response = 
            await baseService.DeleteAsync<bool?>(
                ApiEndpoints.TrainingFormat.ActivateDeactivateTrainingFormat, DeleteType.Patch, pathParameter);
        
        return response;
    }

    public async Task<ResponseDto<bool?>?> ActivateDeactivateTrainingFormat(Guid trainingFormatId)
    {
        var pathParameter = new List<string>()
        {
            trainingFormatId.ToString()
        };
        
        var response = await baseService.DeleteAsync<bool?>(ApiEndpoints.TrainingFormat.ActivateDeactivateTrainingFormat, DeleteType.Patch, pathParameter);

        return response;
    }
}