using CMSTrain.Domain.Common;
using CMSTrain.Domain.Entities;
using CMSTrain.Application.Common.API;
using CMSTrain.Application.Exceptions;
using CMSTrain.Application.DTOs.Country;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.Interfaces.Repositories.Base;

namespace CMSTrain.Infrastructure.Implementation.Services;

public class CountryService(IGenericRepository genericRepository, IApiClientService apiClientService) : ICountryService
{
    public List<GetCountryDto> GetAllCountries(int pageNumber, int pageSize, out int rowCount, string? search = null, bool? isActive = null)
    {
        var countries = genericRepository.GetPagedResult<Country>(pageNumber, pageSize, out rowCount, 
            x => (string.IsNullOrEmpty(search) || x.Name.ToLower().Contains(search.ToLower()) || x.Code.ToLower().Contains(search.ToLower()))
            && (isActive == null || x.IsActive == isActive)).ToList();
        
        return countries.Select(x => new GetCountryDto()
        {
            Id = x.Id,
            Code = x.Code,
            Icon = x.Icon,
            Name = x.Name,
            PhoneCode = x.PhoneCode,
            IsActive = x.IsActive
        }).ToList();
    }
    
    public List<GetCountryDto> GetAllCountries(string? search = null, bool? isActive = null)
    {
        var countries = genericRepository.Get<Country>(x => (string.IsNullOrEmpty(search) || x.Name.ToLower().Contains(search.ToLower()) || x.Code.ToLower().Contains(search.ToLower()))
                                                            && (isActive == null || x.IsActive == isActive)).ToList();

        return countries.Select(country => new GetCountryDto
        {
            Id = country.Id,
            Code = country.Code,
            Icon = country.Icon,
            Name = country.Name,
            PhoneCode = country.PhoneCode,
            IsActive = country.IsActive
        }).ToList();
    }

    public async Task<List<GetCountryDto>> GetGlobalCountries()
    {
        var parameter = new Dictionary<string, string>()
        {
            { "limit", "250" }
        };

        var existingCountries = GetAllCountries();
        
        var existingCountryCodes = existingCountries.Select(country => country.Code).ToHashSet();
        
        var response = await apiClientService.GetAsync<GetGlobalCountryDto>("https://api.first.org/data/v1/countries", parameter);

        if (response != null)
        {
            var newCountries = response.Data.Where(x => !existingCountryCodes.Contains(x.Key)).Select(x => x.Value).OrderBy(x => x.Country);
            
            return newCountries.Select(x => new GetCountryDto()
            {
                Name = x.Country,
                Code = response.Data.FirstOrDefault(z => z.Value.Country == x.Country).Key
            }).ToList();
        }
        
        return new List<GetCountryDto>();
    }

    public GetCountryDto GetCountryById(Guid id)
    {
        var country = genericRepository.GetById<Country>(id)
            ?? throw new NotFoundException("The following country was not found.");

        var result = new GetCountryDto()
        {
            Id = country.Id,
            Code = country.Code,
            Icon = country.Icon,
            Name = country.Name,
            PhoneCode = country.PhoneCode,
            IsActive = country.IsActive
        };

        return result;
    }

    public GetCountryDto GetDefaultCountry()
    {
        var country = genericRepository.GetFirstOrDefault<Country>(x => x.Code == "NP")
                      ?? throw new NotFoundException("The following country was not found.");

        var result = new GetCountryDto()
        {
            Id = country.Id,
            Code = country.Code,
            Icon = country.Icon,
            Name = country.Name,
            PhoneCode = country.PhoneCode,
            IsActive = country.IsActive
        };

        return result;
    }
    
    public void InsertCountry(CreateCountryDto country)
    {
        var existingCountry = genericRepository.GetFirstOrDefault<Country>(x => x.Name == country.Name);

        if (existingCountry != null) 
        {
            throw new NotFoundException("The following country already exists.");
        }

        var countryModel = new Country()
        {
            Name = country.Name,
            PhoneCode = country.PhoneCode,
            Code = country.Code,
            Icon = $"{Constants.CountryFlag.Url}/{country.Code}.svg",
            IsActive = true
        };
        
        genericRepository.Insert(countryModel);
    }

    public void UpdateCountry(UpdateCountryDto country)
    {
        var countryModel = genericRepository.GetById<Country>(country.Id)
            ?? throw new NotFoundException("The following country with specified Id not found.");

        countryModel.Name = country.Name;
        countryModel.PhoneCode = country.PhoneCode;
        countryModel.Code = country.Code;
        countryModel.Icon = $"{Constants.CountryFlag.Url}/{country.Code}.svg";

        genericRepository.Update(countryModel);
    }

    public void ActivateDeactivateCountry(Guid id)
    {
        var countryModel = genericRepository.GetById<Country>(id)
            ?? throw new NotFoundException("The following country with specified Id not found.");

        countryModel.IsActive = !countryModel.IsActive; 
        
        genericRepository.Update(countryModel);
    }
}
