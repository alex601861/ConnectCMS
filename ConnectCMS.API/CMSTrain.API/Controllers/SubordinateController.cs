using CMSTrain.Domain.Common.Enum;
using CMSTrain.Application.Common.Response;
using CMSTrain.Application.DTOs.Subordinate;
using CMSTrain.Application.Interfaces.Services;

namespace CMSTrain.Controllers;

[Route("api/subordinate")]
public class SubordinateController(ISubordinateService subordinateService) : BaseController<SubordinateController>
{
    [HttpGet("details/{subordinateId:guid}")]
    public IActionResult GetSubordinateById(Guid subordinateId)
    {
        var result = subordinateService.GetSubordinateById(subordinateId);

        return Ok(new ResponseDto<GetSubordinateDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Subordinate of provided identifier successfully fetched.",
            Result = result
        });
    }

    [HttpGet("{trainingId:guid}")]
    public IActionResult GetSubordinateDetails(Guid trainingId, int pageNumber, int pageSize, string? search, int? type)
    {
        var result = subordinateService.GetSubordinateDetails(trainingId, pageNumber, pageSize, out var rowCount, search, type);

        return Ok(new CollectionDto<GetSubordinateDto>(result, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Certifications successfully retrieved."
        });
    }
    
    [HttpGet("list/{trainingId:guid}")]
    public IActionResult GetSubordinateDetails(Guid trainingId)
    {
        var result = subordinateService.GetSubordinateDetails(trainingId);

        return Ok(new ResponseDto<List<GetSubordinateDto>>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Subordinates of the following training successfully retrieved.",
            Result = result
        });
    }

    [HttpGet("training-candidate/{trainingCandidateId:guid}")]
    public IActionResult GetSubordinateDetailsForTrainingCandidate(Guid trainingCandidateId, int pageNumber, int pageSize)
    {
        var result = subordinateService.GetSubordinateDetailsForTrainingCandidate(trainingCandidateId, pageNumber, pageSize, out var rowCount);

        return Ok(new CollectionDto<GetSubordinateDto>(result, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Certifications successfully retrieved."
        });
    }
    
    [HttpGet("training-candidate/list/{trainingCandidateId:guid}")]
    public IActionResult GetSubordinateDetailsForTrainingCandidate(Guid trainingCandidateId)
    {
        var result = subordinateService.GetSubordinateDetailsForTrainingCandidate(trainingCandidateId);

        return Ok(new ResponseDto<List<GetSubordinateDto>>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Subordinates of the following training successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("{trainingId:guid}/{subordinateType}")]
    public IActionResult GetSubordinateDetails(Guid trainingId, SubordinateType subordinateType)
    {
        var result = subordinateService.GetSubordinateDetails(trainingId, subordinateType);

        return Ok(new ResponseDto<GetSubordinateDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Subordinate of the following training successfully retrieved.",
            Result = result
        });
    }

    [HttpPost("candidate")]
    public IActionResult InsertSubordinateForCandidates(CreateSubordinateDto subordinate)
    {
        subordinateService.InsertSubordinateForCandidates(subordinate);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Subordinates successfully fetched.",
            Result = true
        });
    }
    
    [HttpPost("training-candidate")]
    public IActionResult InsertSubordinateForCandidates(CreateClientSubordinateDto subordinate)
    {
        subordinateService.InsertSubordinateForCandidates(subordinate);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Subordinates successfully fetched.",
            Result = true
        });
    }
}
