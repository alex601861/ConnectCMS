using CMSTrain.Domain.Entities;
using CMSTrain.Domain.Common.Enum;
using CMSTrain.Application.Exceptions;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.DTOs.PersonalityTrait;
using CMSTrain.Application.Interfaces.Repositories.Base;

namespace CMSTrain.Infrastructure.Implementation.Services;

public class PersonalityTraitService(IGenericRepository genericRepository) : IPersonalityTraitService
{
    public List<GetPersonalityTraitDto> GetAllPersonalityTraits(int pageNumber, int pageSize, out int rowCount, string? search)
    {
        var personalityTraits = genericRepository.GetPagedResult<PersonalityTrait>(
            pageNumber, pageSize, out rowCount,
            x =>
                string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search.ToLower())
        ).ToList();

        var result = personalityTraits.Select(x => new GetPersonalityTraitDto()
        {
            Id = x.Id,
            Title = x.Title,
            Description = x.Description,
            Type = x.Type.ToString()
        }).ToList();

        return result;
    }

    public List<GetPersonalityTraitDto> GetAllPersonalityTraits(string? search)
    {
        var result = new List<GetPersonalityTraitDto>();

        var personalityTraits = genericRepository.Get<PersonalityTrait>(x =>
            string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search.ToLower()));

        foreach (var trait in personalityTraits)
        {
            var traitDto = new GetPersonalityTraitDto()
            {
                Id = trait.Id,
                Title = trait.Title,
                Description = trait.Description,
                Type = trait.Type.ToString()
            };

            result.Add(traitDto);
        }

        return result;
    }

    public GetPersonalityTraitDto GetPersonalityTraitById(Guid personalityTraitId)
    {
        var personalityTrait = genericRepository.GetById<PersonalityTrait>(personalityTraitId)
            ?? throw new NotFoundException("The personality trait with the specified Id was not found.");

        var result = new GetPersonalityTraitDto()
        {
            Id = personalityTrait.Id,
            Title = personalityTrait.Title,
            Description = personalityTrait.Description,
            Type = personalityTrait.Type.ToString()
        };

        return result;
    }

    public GetPersonalityTraitDto GetPersonalityTrait(TraitType traitType)
    {
        var personalityTrait = genericRepository.GetFirstOrDefault<PersonalityTrait>(x => x.Type == traitType)
                               ?? throw new NotFoundException("The personality trait with the specified Id was not found.");

        var result = new GetPersonalityTraitDto()
        {
            Id = personalityTrait.Id,
            Title = personalityTrait.Title,
            Description = personalityTrait.Description,
            Type = personalityTrait.Type.ToString()
        };

        return result;
    }
    
    public void UpdatePersonalityTrait(UpdatePersonalityTraitDto personalityTrait)
    {
        var personalityTraitModel = genericRepository.GetById<PersonalityTrait>(personalityTrait.Id)
            ?? throw new NotFoundException("The specified personality trait was not found.");

        personalityTraitModel.Description = personalityTrait.Description;

        genericRepository.Update(personalityTraitModel);
    }
}
