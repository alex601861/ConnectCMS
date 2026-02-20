using CMSTrain.Application.Common.Response;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.DTOs.PersonalityTrait;
using CMSTrain.Domain.Common.Enum;

namespace CMSTrain.Controllers;

[Route("api/personality-trait")]
public class PersonalityTraitController(IPersonalityTraitService personalityTraitService) : BaseController<PersonalityTestController>
{
    [HttpGet]
    public IActionResult GetAllPersonalityTraits(int pageNumber, int pageSize, string? search)
    {
        var personalityTraits = personalityTraitService.GetAllPersonalityTraits(pageNumber, pageSize, out var rowCount, search);

        return Ok(new CollectionDto<GetPersonalityTraitDto>(personalityTraits, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Personality traits successfully retrieved."
        });
    }

    [HttpGet("list")]
    public IActionResult GetAllPersonalityTraits(string? search)
    {
        var result = personalityTraitService.GetAllPersonalityTraits(search);

        return Ok(new ResponseDto<List<GetPersonalityTraitDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Personality traits successfully retrieved.",
            Result = result
        });
    }

    [HttpGet("{personalityTraitId:guid}")]
    public IActionResult GetPersonalityTraitById(Guid personalityTraitId)
    {
        var result = personalityTraitService.GetPersonalityTraitById(personalityTraitId);

        return Ok(new ResponseDto<GetPersonalityTraitDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Personality trait successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("{traitType}")]
    public IActionResult GetPersonalityTrait(TraitType traitType)
    {
        var result = personalityTraitService.GetPersonalityTrait(traitType);

        return Ok(new ResponseDto<GetPersonalityTraitDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Personality trait successfully retrieved.",
            Result = result
        });
    }

    [HttpPut]
    public IActionResult UpdatePersonalityTrait(UpdatePersonalityTraitDto personalityTrait)
    {
        personalityTraitService.UpdatePersonalityTrait(personalityTrait);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Personality trait successfully updated.",
            Result = true
        });
    }
}