using System.Text.Json;
using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Interface;
using CMSTrain.Client.Models.Requests.PersonalityTrait;
using CMSTrain.Client.Models.Responses.PersonalityTrait;

namespace CMSTrain.Client.Service.Implementation;

public class PersonalityTraitService(IBaseService baseService) : IPersonalityTraitService
{
    public async Task<CollectionDto<GetPersonalityTraitDto>?> GetAllPersonalityTraits(int pageNumber, int pageSize, string? search = null)
    {
        var queryParameter = new Dictionary<string, string?>()
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search }
        };

        var response = await baseService.GetPagedAsync<GetPersonalityTraitDto>(endpoint: ApiEndpoints.PersonalityTrait.GetAllPersonalityTraits, parameters: queryParameter);

        return response;
    }

    public async Task<ResponseDto<List<GetPersonalityTraitDto>?>?> GetAllPersonalityTraits(string? search = null)
    {
        var queryParameter = new Dictionary<string, string?>()
        {
            { "search", search }
        };

        var response = await baseService.GetAsync<List<GetPersonalityTraitDto>?>(endpoint: ApiEndpoints.PersonalityTrait.GetAllPersonalityTraitsList, parameters: queryParameter);

        return response;
    }

    public async Task<ResponseDto<GetPersonalityTraitDto?>?> GetPersonalityTraitById(Guid personalityTraitId)
    {
        var parameter = new List<string>()
        {
            personalityTraitId.ToString()
        };

        var response = await baseService.GetAsync<GetPersonalityTraitDto?>(endpoint: ApiEndpoints.PersonalityTrait.GetPersonalityTraitById, parameter);

        return response;
    }

    public async Task<ResponseDto<GetPersonalityTraitDto?>?> GetPersonalityTrait(TraitType traitType)
    {
        var parameter = new List<string>()
        {
            traitType.ToString()
        };

        var response = await baseService.GetAsync<GetPersonalityTraitDto?>(endpoint: ApiEndpoints.PersonalityTrait.GetPersonalityTrait, parameter);

        return response;
    }
    
    public async Task<ResponseDto<bool?>?> UpdatePersonalityTrait(UpdatePersonalityTraitDto personalityTrait)
    {
        var jsonRequest = JsonSerializer.Serialize(personalityTrait);
        
        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

        var response = await baseService.UpdateAsync<bool?>(ApiEndpoints.PersonalityTrait.UpdatePersonalityTrait, Constants.UpdateType.Put, content);

        return response;
    }
}