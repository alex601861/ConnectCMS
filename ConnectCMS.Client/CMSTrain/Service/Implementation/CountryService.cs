using System.Text.Json;
using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Interface;
using CMSTrain.Client.Models.Requests.Country;
using CMSTrain.Client.Models.Responses.Country;

namespace CMSTrain.Client.Service.Implementation;

public class CountryService(IBaseService baseService) : ICountryService
{
    public async Task<CollectionDto<GetCountryDto>?> GetAllCountries(int pageNumber, int pageSize, string? search = null, bool? isActive = null)
    {
        var queryParameter = new Dictionary<string, string?>()
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search },
            { "isActive", isActive.ToString() }
        };

        var response = await baseService.GetPagedAsync<GetCountryDto>(endpoint: ApiEndpoints.Country.GetAllCountries, parameters: queryParameter);

        return response;
    }
    
    public async Task<ResponseDto<List<GetCountryDto>?>?> GetAllCountries(string? search = null, bool? isActive = null)
    {
        var queryParameter = new Dictionary<string, string?>()
        {
            { "search", search },
            { "isActive", isActive.ToString() }
        };
        
        var response = await baseService.GetAsync<List<GetCountryDto>?>(ApiEndpoints.Country.GetAllCountriesList, parameters: queryParameter);

        return response;
    }

    public async Task<ResponseDto<GetCountryDto?>?> GetCountryById(Guid countryId)
    {
        var pathParameter = new List<string>
        {
            countryId.ToString()
        };
        
        var response = await baseService.GetAsync<GetCountryDto?>(ApiEndpoints.Country.GetCountryById, pathParameter);

        return response;
    }

    public async Task<ResponseDto<GetCountryDto?>?> GetDefaultCountry()
    {
        var response = await baseService.GetAsync<GetCountryDto?>(ApiEndpoints.Country.GetDefaultCountry);

        return response;
    }
    
    public async Task<ResponseDto<List<GetCountryDto>?>?> GetGlobalCountries()
    {
        var response = await baseService.GetAsync<List<GetCountryDto>>(ApiEndpoints.Country.GetGlobalCountries);

        return response;
        
    }
    
    public async Task<ResponseDto<bool?>?> InsertCountry(CreateCountryDto country)
    {
        var jsonRequest = JsonSerializer.Serialize(country);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.PostAsync<bool?>(ApiEndpoints.Country.InsertCountry, content);
        
        return response;
    }

    public async Task<ResponseDto<bool?>?> UpdateCountry(UpdateCountryDto country)
    {
        var jsonRequest = JsonSerializer.Serialize(country);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.UpdateAsync<bool?>(ApiEndpoints.Country.UpdateCountry, Constants.UpdateType.Put, content);
        
        return response;
    }

    public async Task<ResponseDto<bool?>?> ActivateDeactivateCountry(Guid countryId)
    {
        var pathParameter = new List<string>
        {
            countryId.ToString()
        };
        
        var response = await baseService.DeleteAsync<bool?>(ApiEndpoints.Country.ActivateDeactivateCountry, Constants.DeleteType.Patch, pathParameter);

        return response;
    }
}