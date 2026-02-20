using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Dependency;
using CMSTrain.Client.Models.Requests.PersonalityTrait;
using CMSTrain.Client.Models.Responses.PersonalityTrait;

namespace CMSTrain.Client.Service.Interface;

public interface IPersonalityTraitService : ITransientService
{
    Task<CollectionDto<GetPersonalityTraitDto>?> GetAllPersonalityTraits(int pageNumber, int pageSize, string? search = null);
    
    Task<ResponseDto<List<GetPersonalityTraitDto>?>?> GetAllPersonalityTraits(string? search = null);

    Task<ResponseDto<GetPersonalityTraitDto?>?> GetPersonalityTraitById(Guid personalityTraitId);

    Task<ResponseDto<GetPersonalityTraitDto?>?> GetPersonalityTrait(TraitType traitType);
    
    Task<ResponseDto<bool?>?> UpdatePersonalityTrait(UpdatePersonalityTraitDto personalityTrait);
}