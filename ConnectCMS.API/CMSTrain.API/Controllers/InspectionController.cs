using CMSTrain.Application.DTOs.Inspection;
using CMSTrain.Application.Common.Response;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Domain.Common.Enum;

namespace CMSTrain.Controllers;

[Route("api/inspection")]
public class InspectionController(IInspectionService inspectionService) : BaseController<CountryController>
{
    [HttpGet]
    public IActionResult GetAllInspections(int pageNumber, int pageSize, string? search, bool? isActive)
    {
        var inspections = inspectionService.GetAllInspections(pageNumber, pageSize, out var rowCount, search, isActive);

        return Ok(new CollectionDto<GetInspectionDto>(inspections, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Inspections successfully retrieved."
        });
    }
    
    [HttpGet("list")]
    public IActionResult GetAllInspections(string? search, bool? isActive)
    {
        var inspections = inspectionService.GetAllInspections(search, isActive);

        return Ok(new ResponseDto<List<GetInspectionDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Inspections successfully retrieved.",
            Result = inspections
        });
    }
    
    [HttpGet("{inspectionId:guid}")]
    public IActionResult GetInspectionById(Guid inspectionId)
    {
        var inspection = inspectionService.GetInspectionById(inspectionId);

        return Ok(new ResponseDto<GetInspectionDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Inspection successfully retrieved.",
            Result = inspection
        });
    }
    
    [HttpGet("{inspectionType}")]
    public IActionResult GetInspectionByType(InspectionType inspectionType)
    {
        var inspection = inspectionService.GetInspectionByType(inspectionType);

        return Ok(new ResponseDto<GetInspectionDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Inspection successfully retrieved.",
            Result = inspection
        });
    }
    
    [HttpGet("available")]
    public IActionResult GetAllAvailableTrainingInspections(int pageNumber, int pageSize)
    {
        var inspection = inspectionService.GetAllAvailableTrainingInspections(pageNumber, pageSize, out var rowCount);

        return Ok(new CollectionDto<GetInspectionDto>(inspection, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Inspections successfully retrieved."
        });
    }
    
    [HttpGet("available/list")]
    public IActionResult GetAllAvailableTrainingInspections()
    {
        var inspection = inspectionService.GetAllAvailableTrainingInspections();

        return Ok(new ResponseDto<List<GetInspectionDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Inspection successfully retrieved.",
            Result = inspection
        });
    }

    [HttpGet("assigned/{trainingId:guid}")]
    public IActionResult GetAllAssignedTrainingInspections(Guid trainingId, int pageNumber, int pageSize, string? search)
    {
        var inspection = inspectionService.GetAllAssignedTrainingInspections(trainingId, pageNumber, pageSize, out var rowCount, search);

        return Ok(new CollectionDto<GetInspectionDto>(inspection, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Inspections successfully retrieved."
        });
    }
    
    [HttpGet("assigned/list/{trainingId:guid}")]
    public IActionResult GetAllAssignedTrainingInspections(Guid trainingId, string? search)
    {
        var inspection = inspectionService.GetAllAssignedTrainingInspections(trainingId, search);

        return Ok(new ResponseDto<List<GetInspectionDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Inspections successfully retrieved.",
            Result = inspection
        });
    }
    
    [HttpPost]
    public IActionResult InsertInspections([FromForm] CreateInspectionDto inspection)
    {
        inspectionService.InsertInspection(inspection);
        
        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Inspection successfully created.",
            Result = true
        });
    }
    
    [HttpPut]
    public IActionResult UpdateInspections([FromForm] UpdateInspectionDto inspection)
    {
        inspectionService.UpdateInspection(inspection);
        
        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Inspection successfully updated.",
            Result = true
        });
    }
    
    [HttpPatch("{inspectionId:guid}")]
    public IActionResult ActivateDeactivateInspection(Guid inspectionId)
    {
        inspectionService.ActivateDeactivateInspection(inspectionId);
        
        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "The status of inspection successfully updated.",
            Result = true
        });
    }

    [HttpPost("questionnaires")]
    public IActionResult UploadInspectionQuestionnaires(UploadInspectionQuestionnaireDto inspectionQuestionnaires)
    {
        inspectionService.UploadInspectionQuestionnaires(inspectionQuestionnaires);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Inspection questionnaires successfully uploaded.",
            Result = true
        });
    }
}