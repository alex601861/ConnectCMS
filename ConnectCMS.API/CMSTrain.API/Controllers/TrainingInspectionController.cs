using CMSTrain.Application.Common.Response;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.DTOs.TrainingInspection;

namespace CMSTrain.Controllers;

[Route("api/training-inspection")]
public class TrainingInspectionController(ITrainingInspectionService trainingInspectionService) : BaseController<TrainingInspectionController>
{
    [HttpGet("details/{trainingInspectionId:guid}")]
    public IActionResult GetTrainingInspectionById(Guid trainingInspectionId)
    {
        var inspections = trainingInspectionService.GetTrainingInspectionById(trainingInspectionId);

        return Ok(new ResponseDto<GetTrainingInspectionDetailsDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Assigned training inspection successfully retrieved.",
            Result = inspections
        });
    }
    
    [HttpGet("questionnaire/details/{questionnaireId:guid}")]
    public IActionResult GetTrainingInspectionByQuestionnaire(Guid questionnaireId)
    {
        var inspections = trainingInspectionService.GetTrainingInspectionByQuestionnaire(questionnaireId);

        return Ok(new ResponseDto<GetTrainingInspectionDetailsDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Assigned training inspection successfully retrieved.",
            Result = inspections
        });
    }
    
    [HttpGet("{trainingId:guid}")]
    public IActionResult GetAllTrainingInspections(Guid trainingId, int pageNumber, int pageSize, string? search)
    {
        var inspections = trainingInspectionService.GetAllAssignedTrainingInspections(trainingId, pageNumber, pageSize, out var rowCount, search);

        return Ok(new CollectionDto<GetTrainingInspectionDto>(inspections, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Assigned training inspections successfully retrieved."
        });
    }
    
    [HttpGet("list/{trainingId:guid}")]
    public IActionResult GetAllTrainingInspections(Guid trainingId, string? search)
    {
        var inspections = trainingInspectionService.GetAllAssignedTrainingInspections(trainingId, search);

        return Ok(new ResponseDto<List<GetTrainingInspectionDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Assigned training inspections successfully retrieved.",
            Result = inspections
        });
    }

    [HttpGet("candidate/{trainingId:guid}")]
    public IActionResult GetAllAssignedTrainingInspectionsForCandidate(Guid trainingId, int pageNumber, int pageSize, string? search)
    {
        var result = trainingInspectionService.GetAllAssignedTrainingInspectionsForCandidateAndClient(trainingId, pageNumber, pageSize, out var rowCount,  search);

        return Ok(new CollectionDto<GetTrainingInspectionDto>(result, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Assigned training inspections successfully retrieved."
        });
    }
    
    [HttpGet("candidate/list/{trainingId:guid}")]
    public IActionResult GetAllAssignedTrainingInspectionsForCandidate(Guid trainingId, string? search)
    {
        var inspections = trainingInspectionService.GetAllAssignedTrainingInspectionsForCandidateAndClient(trainingId, search);

        return Ok(new ResponseDto<List<GetTrainingInspectionDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Assigned training inspections successfully retrieved.",
            Result = inspections
        });
    }
    
    [HttpGet("client/{trainingId:guid}")]
    public IActionResult GetAllAssignedTrainingInspectionsForClient(Guid trainingId, int pageNumber, int pageSize, string? search)
    {
        var result = trainingInspectionService.GetAllAssignedTrainingInspectionsForCandidateAndClient(trainingId, pageNumber, pageSize, out var rowCount,  search);

        return Ok(new CollectionDto<GetTrainingInspectionDto>(result, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Assigned training inspections successfully retrieved."
        });
    }
    
    [HttpGet("client/list/{trainingId:guid}")]
    public IActionResult GetAllAssignedTrainingInspectionsForClient(Guid trainingId, string? search)
    {
        var inspections = trainingInspectionService.GetAllAssignedTrainingInspectionsForCandidateAndClient(trainingId, search);

        return Ok(new ResponseDto<List<GetTrainingInspectionDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Assigned training inspections successfully retrieved.",
            Result = inspections
        });
    }
    
    [HttpGet("training-candidate/{trainingCandidateId:guid}")]
    public IActionResult GetAllAssignedTrainingInspectionsForTrainingCandidate(Guid trainingCandidateId, int pageNumber, int pageSize, string? search)
    {
        var result = trainingInspectionService.GetAllAssignedTrainingInspectionsForTrainingCandidate(trainingCandidateId, pageNumber, pageSize, out var rowCount,  search);

        return Ok(new CollectionDto<GetTrainingInspectionDto>(result, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Assigned training inspections successfully retrieved."
        });
    }
    
    [HttpGet("training-candidate/list/{trainingCandidateId:guid}")]
    public IActionResult GetAllAssignedTrainingInspectionsForTrainingCandidate(Guid trainingCandidateId, string? search)
    {
        var inspections = trainingInspectionService.GetAllAssignedTrainingInspectionsForTrainingCandidate(trainingCandidateId, search);

        return Ok(new ResponseDto<List<GetTrainingInspectionDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Assigned training inspections successfully retrieved.",
            Result = inspections
        });
    }
    
    [HttpGet("candidate/details/{trainingInspectionId:guid}")]
    public IActionResult GetCandidateTrainingInspectionDetails(Guid trainingInspectionId)
    {
        var inspections = trainingInspectionService.GetCandidateTrainingInspectionDetails(trainingInspectionId);

        return Ok(new ResponseDto<GetCandidateTrainingInspectionDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Assigned training inspections successfully retrieved.",
            Result = inspections
        });
    }
    
    [HttpGet("subordinate/details/{subordinateId:guid}")]
    public IActionResult GetSubordinateTrainingInspectionDetails(Guid subordinateId)
    {
        var inspections = trainingInspectionService.GetSubordinateTrainingInspectionDetails(subordinateId);

        return Ok(new ResponseDto<GetSubordinateTrainingInspectionDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Assigned training inspections successfully retrieved.",
            Result = inspections
        });
    }
    
    [HttpGet("training-candidate/details/{trainingCandidateId:guid}/{trainingInspectionId:guid}")]
    public IActionResult GetCandidateTrainingInspectionDetailsForTrainingCandidate(Guid trainingCandidateId, Guid trainingInspectionId)
    {
        var inspections = trainingInspectionService.GetCandidateTrainingInspectionDetailsForTrainingCandidate(trainingCandidateId, trainingInspectionId);

        return Ok(new ResponseDto<GetCandidateTrainingInspectionDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Assigned training inspections successfully retrieved.",
            Result = inspections
        });
    }
    
    [HttpGet("questionnaires/count/{trainingId:guid}")]
    public IActionResult GetTrainingInspectionQuestionnairesCount(Guid trainingId)
    {
        var result = trainingInspectionService.GetTrainingInspectionQuestionnairesCount(trainingId);
    
        return Ok(new ResponseDto<GetTrainingInspectionQuestionnaireCountDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Assigned training inspection questionnaires successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("count/{trainingInspectionId:guid}")]
    public IActionResult GetTrainingInspectionPhaseCounts(Guid trainingInspectionId)
    {
        var result = trainingInspectionService.GetTrainingInspectionPhaseCounts(trainingInspectionId);
    
        return Ok(new ResponseDto<int>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Assigned training inspection questionnaires successfully retrieved.",
            Result = result
        });
    }

    [HttpPost]
    public IActionResult AssignTrainingInspections(AssignTrainingInspectionDto trainingInspections)
    {
        trainingInspectionService.AssignTrainingInspections(trainingInspections);
        
        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Inspection successfully assigned to the following training.",
            Result = true
        });
    }
}