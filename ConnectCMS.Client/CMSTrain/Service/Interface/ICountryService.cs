using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Dependency;
using CMSTrain.Client.Models.Requests.Country;
using CMSTrain.Client.Models.Responses.Country;

namespace CMSTrain.Client.Service.Interface;

public interface ICountryService : ITransientService
{
    Task<CollectionDto<GetCountryDto>?> GetAllCountries(int pageNumber, int pageSize, string? search = null, bool? isActive = null);
    
    Task<ResponseDto<List<GetCountryDto>?>?> GetAllCountries(string? search = null, bool? isActive = null);

    Task<ResponseDto<GetCountryDto?>?> GetCountryById(Guid countryId);
    
    Task<ResponseDto<GetCountryDto?>?> GetDefaultCountry();
    
    Task<ResponseDto<List<GetCountryDto>?>?> GetGlobalCountries();

    Task<ResponseDto<bool?>?> InsertCountry(CreateCountryDto country);

    Task<ResponseDto<bool?>?> UpdateCountry(UpdateCountryDto country);

    Task<ResponseDto<bool?>?> ActivateDeactivateCountry(Guid countryId);
}