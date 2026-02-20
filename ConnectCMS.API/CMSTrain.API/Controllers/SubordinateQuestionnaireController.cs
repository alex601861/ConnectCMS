using CMSTrain.Application.Common.Response;
using CMSTrain.Application.DTOs.Candidate;
using CMSTrain.Application.DTOs.Inspection;
using CMSTrain.Application.DTOs.Questionnaires;
using CMSTrain.Application.DTOs.Subordinate;
using CMSTrain.Application.DTOs.Training;
using CMSTrain.Application.DTOs.TrainingInspection;
using CMSTrain.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;

namespace CMSTrain.Controllers;

[AllowAnonymous]
[Route("api/subordinate-questionnaire")]
public class SubordinateQuestionnaireController(ITrainingService trainingService,
    IInspectionService inspectionService, 
    ITrainingInspectionService trainingInspectionService, 
    ISubordinateService subordinateService,
    IQuestionnaireService questionnaireService) : BaseController<SubordinateQuestionnaireController>
{
    [HttpGet("training/details/{trainingId:guid}")]
    public IActionResult GetTrainingById(Guid trainingId)
    {
        var result = trainingService.GetTrainingById(trainingId);

        return Ok(new ResponseDto<GetTrainingDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Training successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("inspection/details/{inspectionId:guid}")]
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
    
    [HttpGet("subordinate/details/{subordinateId:guid}")]
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
    
    [HttpGet("candidate/details/{subordinateId:guid}")]
    public IActionResult GetCandidateBySubordinateId(Guid subordinateId)
    {
        var result = subordinateService.GetCandidateBySubordinateId(subordinateId);

        return Ok(new ResponseDto<GetCandidateDetailsDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Subordinate of provided identifier successfully fetched.",
            Result = result
        });
    }
    
    [HttpGet("training/inspection/details/{trainingInspectionId:guid}")]
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
    
    [HttpGet("details/{questionnaireId:guid}/{subordinateId:guid}")]
    public IActionResult GetAllQuestionnairesForSubordinates(Guid questionnaireId, Guid subordinateId)
    {
        var result = questionnaireService.GetAllQuestionnairesForSubordinates(questionnaireId, subordinateId);

        return Ok(new ResponseDto<GetCandidateQuestionnaireDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Available questions are successfully fetched.",
            Result = result
        });
    }
}