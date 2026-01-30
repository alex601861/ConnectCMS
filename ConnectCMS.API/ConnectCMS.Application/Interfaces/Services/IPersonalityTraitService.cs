using CMSTrain.Domain.Common.Enum;
using CMSTrain.Application.Common.Service;
using CMSTrain.Application.DTOs.PersonalityTrait;

namespace CMSTrain.Application.Interfaces.Services;

public interface IPersonalityTraitService : ITransientService
{
    List<GetPersonalityTraitDto> GetAllPersonalityTraits(int pageNumber, int pageSize, out int rowCount, string? search);

    List<GetPersonalityTraitDto> GetAllPersonalityTraits(string? search);

    GetPersonalityTraitDto GetPersonalityTraitById(Guid personalityTraitId);

    GetPersonalityTraitDto GetPersonalityTrait(TraitType traitType);

    void UpdatePersonalityTrait(UpdatePersonalityTraitDto personalityTrait);
}
