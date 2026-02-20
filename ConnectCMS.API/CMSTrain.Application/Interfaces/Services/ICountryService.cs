using CMSTrain.Application.Common.Service;
using CMSTrain.Application.DTOs.Country;

namespace CMSTrain.Application.Interfaces.Services;

public interface ICountryService : ITransientService
{
    List<GetCountryDto> GetAllCountries(int pageNumber, int pageSize, out int rowCount, string? search = null, bool? isActive = null);

    List<GetCountryDto> GetAllCountries(string? search = null, bool? isActive = null);

    Task<List<GetCountryDto>> GetGlobalCountries();
    
    GetCountryDto GetCountryById(Guid id);

    GetCountryDto GetDefaultCountry();
    
    void InsertCountry(CreateCountryDto country);

    void UpdateCountry(UpdateCountryDto country);

    void ActivateDeactivateCountry(Guid id);
}