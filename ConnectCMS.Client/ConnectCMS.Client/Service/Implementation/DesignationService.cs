using System.Text.Json;
using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Requests.Designation;
using CMSTrain.Client.Models.Responses.Designation;
using CMSTrain.Client.Service.Base;
using CMSTrain.Client.Service.Interface;

namespace CMSTrain.Client.Service.Implementation;

public class DesignationService(IBaseService baseService) :  IDesignationService
{
    public async Task<CollectionDto<GetDesignationDto>?> GetAllDesignations(int pageNumber, int pageSize, string? search = null, bool? isActive = null)
    {
        var queryParameter = new Dictionary<string, string?>()
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search },
            { "isActive", isActive.ToString() }
        };

        var response = await baseService.GetPagedAsync<GetDesignationDto>(endpoint: ApiEndpoints.Designation.GetAllDesignation, parameters: queryParameter);

        return response;
    }

    public async Task<ResponseDto<List<GetDesignationDto>?>?> GetAllDesignations(string? search = null, bool? isActive = null)
    {
        var queryParameter = new Dictionary<string, string?>()
        {
            { "search", search },
            { "isActive", isActive.ToString() }
        };
        
        var response = await baseService.GetAsync<List<GetDesignationDto>?>(ApiEndpoints.Designation.GetAllDesignationsList, parameters: queryParameter);

        return response;
    }

    public async Task<ResponseDto<GetDesignationDto?>?> GetDesignationById(Guid countryId)
    {
        var pathParameter = new List<string>
        {
            countryId.ToString()
        };
        
        var response = await baseService.GetAsync<GetDesignationDto?>(ApiEndpoints.Designation.GetDesignationById, pathParameter);

        return response;
    }

    public async Task<ResponseDto<bool?>?> InsertDesignation(CreateDesignationDto designation)
    {
        var jsonRequest = JsonSerializer.Serialize(designation);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.PostAsync<bool?>(ApiEndpoints.Designation.InsertDesignation, content);
        
        return response;
    }

    public async Task<ResponseDto<bool?>?> UpdateDesignation(UpdateDesignationDto designation)
    {
        var jsonRequest = JsonSerializer.Serialize(designation);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.UpdateAsync<bool?>(ApiEndpoints.Designation.UpdateDesignation, Constants.UpdateType.Put, content);
        
        return response;
    }

    public async Task<ResponseDto<bool?>?> ActivateDeactivateDesignation(Guid designationId)
    {
        var pathParameter = new List<string>
        {
            designationId.ToString()
        };
        
        var response = await baseService.DeleteAsync<bool?>(ApiEndpoints.Designation.ActivateDeactivateDesignation, Constants.DeleteType.Patch, pathParameter);

        return response;
    }
}