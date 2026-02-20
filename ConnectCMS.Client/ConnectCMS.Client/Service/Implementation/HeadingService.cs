using System.Text.Json;
using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Interface;
using CMSTrain.Client.Models.Requests.Heading;
using CMSTrain.Client.Models.Responses.Heading;

namespace CMSTrain.Client.Service.Implementation;

public class HeadingService(IBaseService baseService) : IHeadingService
{
    public async Task<CollectionDto<GetHeadingDto>?> GetAllHeadings(HeadingType headingType, FacetType facetType, InspectionType inspectionType, int pageNumber, int pageSize, bool? isActive = null,string? search = null)
    {
        var pathParameter = new List<string>
        {
            headingType.ToString(),
            facetType.ToString(),
            inspectionType.ToString()
        };

        var queryParameter = new Dictionary<string, string?>()
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "isActive", isActive.ToString() },
            { "search", search }
        };
        
        var response = await baseService.GetPagedAsync<GetHeadingDto>(endpoint: ApiEndpoints.Heading.GetAllHeadings, pathParameter, parameters: queryParameter);

        return response;
    }

    public async Task<ResponseDto<List<GetHeadingDto>?>?> GetAllHeadings(HeadingType headingType, FacetType facetType, InspectionType inspectionType, bool? isActive = null,string? search = null)
    {
        var pathParameter = new List<string>
        {
            headingType.ToString(),
            facetType.ToString(),
            inspectionType.ToString()
        };
        
        var queryParameter = new Dictionary<string, string?>()
        {
            { "isActive", isActive.ToString() },
            { "search", search }
        };
        
        var response = await baseService.GetAsync<List<GetHeadingDto>?>(ApiEndpoints.Heading.GetAllHeadingsList, pathParameter, queryParameter);

        return response;
    }

    public async Task<ResponseDto<List<GetHeadingModuleDto>?>?> GetAllParentHeadings(FacetType facetType, InspectionType inspectionType)
    {
        var pathParameter = new List<string>
        {
            facetType.ToString(),
            inspectionType.ToString()
        };

        var response = await baseService.GetAsync<List<GetHeadingModuleDto>?>(ApiEndpoints.Heading.GetAllParentHeadings, pathParameter);

        return response;
    }

    public async Task<ResponseDto<List<GetHeadingModuleDto>?>?> GetAllSubHeadings()
    {
        var response = await baseService.GetAsync<List<GetHeadingModuleDto>?>(ApiEndpoints.Heading.GetAllSubHeadings);

        return response;
    }

    public async Task<ResponseDto<GetHeadingDto?>?> GetHeadingById(Guid headingId)
    {
        var pathParameter = new List<string>
        {
            headingId.ToString()
        };
        
        var response = await baseService.GetAsync<GetHeadingDto?>(ApiEndpoints.Heading.GetHeadingById, pathParameter);

        return response;
    }

    public async Task<ResponseDto<GetHeadingCountDto?>?> GetAllHeadingCount(FacetType facetType, InspectionType inspectionType)
    {
        var pathParameter = new List<string>
        {
            facetType.ToString(),
            inspectionType.ToString()
        };
        
        var response = await baseService.GetAsync<GetHeadingCountDto?>(ApiEndpoints.Heading.GetHeadingCount, pathParameter);

        return response;
    }
    
    public async Task<ResponseDto<bool?>?> InsertHeading(CreateHeadingDto heading)
    {
        var jsonRequest = JsonSerializer.Serialize(heading);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.PostAsync<bool?>(ApiEndpoints.Heading.InsertHeading, content);
        
        return response;
    }

    public async Task<ResponseDto<bool?>?> UpdateHeading(UpdateHeadingDto heading)
    {
        var jsonRequest = JsonSerializer.Serialize(heading);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.UpdateAsync<bool?>(ApiEndpoints.Heading.UpdateHeading, Constants.UpdateType.Put, content);
        
        return response;
    }

    public async Task<ResponseDto<bool?>?> ActivateDeactivateHeading(Guid headingId)
    {
        var pathParameter = new List<string>
        {
            headingId.ToString()
        };
        
        var response = await baseService.DeleteAsync<bool?>(ApiEndpoints.Heading.ActivateDeactivateHeading, Constants.DeleteType.Patch, pathParameter);

        return response;
    }

    public async Task<ResponseDto<bool?>?> DeleteHeading(Guid headingId)
    {
        var pathParameter = new List<string>
        {
            headingId.ToString()
        };
        
        var response = await baseService.DeleteAsync<bool?>(ApiEndpoints.Heading.DeleteHeading, Constants.DeleteType.Delete, pathParameter);

        return response;
    }
}