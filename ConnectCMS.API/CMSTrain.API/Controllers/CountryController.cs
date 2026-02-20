using CMSTrain.Application.DTOs.Country;
using Microsoft.AspNetCore.Authorization;
using CMSTrain.Application.Common.Response;
using CMSTrain.Application.Interfaces.Services;

namespace CMSTrain.Controllers;

[Route("api/country")]
public class CountryController(ICountryService countryService) : BaseController<CountryController>
{
    [HttpGet]
    public IActionResult GetAllCountries(int pageNumber, int pageSize, string? search, bool? isActive)
    {
        var countries = countryService.GetAllCountries(pageNumber, pageSize, out var rowCount, search, isActive);

        return Ok(new CollectionDto<GetCountryDto>(countries, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Countries successfully retrieved.",
            Result = countries
        });
    }

    [AllowAnonymous]
    [HttpGet("list")]
    public IActionResult GetAllCountries(string? search, bool? isActive)
    {
        var result = countryService.GetAllCountries(search, isActive);

        return Ok(new ResponseDto<List<GetCountryDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Country successfully retrieved.",
            Result = result
        });
    }

    [HttpGet("{countryId:guid}")]
    public IActionResult GetCountryById(Guid countryId)
    {
        var result = countryService.GetCountryById(countryId);

        return Ok(new ResponseDto<GetCountryDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Country of provided identifier successfully fetched.",
            Result = result
        });
    }
    
    [HttpGet("default")]
    public IActionResult GetDefaultCountry()
    {
        var result = countryService.GetDefaultCountry();

        return Ok(new ResponseDto<GetCountryDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Country of provided identifier successfully fetched.",
            Result = result
        });
    }

    [HttpGet("global")]
    public async Task<IActionResult> GetGlobalCountries()
    {
        var result = await countryService.GetGlobalCountries();
        
        return Ok(new ResponseDto<List<GetCountryDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Countries successfully fetched.",
            Result = result
        });
    }

    [HttpPost]
    public IActionResult InsertCountry(CreateCountryDto country)
    {
        countryService.InsertCountry(country);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Country successfully created.",
            Result = true
        });
    }

    [HttpPut]
    public IActionResult UpdateCountry(UpdateCountryDto country)
    {
        countryService.UpdateCountry(country);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Country successfully updated.",
            Result = true
        });
    }

    [HttpPatch("{countryId:guid}")]
    public IActionResult ActivateDeactivateCountry(Guid countryId)
    {
        countryService.ActivateDeactivateCountry(countryId);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "The status of country successfully updated.",
            Result = true
        });
    }
}
