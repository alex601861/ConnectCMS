using CMSTrain.Application.DTOs.Candidate;
using CMSTrain.Application.Common.Response;
using CMSTrain.Application.Interfaces.Services;

namespace CMSTrain.Controllers;

[Route("api/candidate")]
public class CandidateController(ICandidateService candidateService) : BaseController<CandidateController>
{
    [HttpGet("{candidateId:guid}")]
    public IActionResult GetCandidateDetailsById(Guid candidateId)
    {
        var candidate = candidateService.GetCandidateDetailsById(candidateId);

        return Ok(new ResponseDto<GetCandidateDetailsDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Candidate details successfully fetched.",
            Result = candidate
        });
    }
}