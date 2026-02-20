using CMSTrain.Application.DTOs.Analysis;
using CMSTrain.Application.Common.Response;
using CMSTrain.Application.Interfaces.Services;

namespace CMSTrain.Controllers;

[Route("api/analysis")]
public class AnalysisController(IAnalysisService analysisService) : BaseController<AnalysisController>
{
    [HttpPost]
    public IActionResult UploadUserResponseAnalysis(UploadUserResponseAnalysisDto userResponseAnalysis)
    {
        analysisService.UploadUserResponseAnalysis(userResponseAnalysis);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Your response analysis have been successfully saved.",
            Result = true
        });
    }

    [HttpGet("feedbacks/{userResponseId:guid}")]
    public IActionResult GetUserResponseAnalysisDetailsForFeedbacks(Guid userResponseId)
    {
        var result = analysisService.GetUserResponseAnalysisDetailsForFeedbacks(userResponseId);
        
        return Ok(new ResponseDto<GetUserResponseAnalysisDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "The respective response analysis has been successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("assessments/{userResponseId:guid}")]
    public IActionResult GetUserResponseAnalysisDetailsForAssessments(Guid userResponseId)
    {
        var result = analysisService.GetUserResponseAnalysisDetailsForAssessments(userResponseId);
        
        return Ok(new ResponseDto<List<GetAssessmentResponseAnalysisDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "The respective response analysis has been successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("assessments/evaluation/{questionnaireId:guid}/{userResponseId:guid}/{isSubordinateRequired:bool}/{phase:int}")]
    public IActionResult GetUserResponseAnalysisEvaluationDetailsForAssessments(Guid questionnaireId, Guid userResponseId, bool isSubordinateRequired, int phase)
    {
        var result = analysisService.GetUserResponseAnalysisEvaluationDetailsForAssessments(questionnaireId, userResponseId, isSubordinateRequired, phase);
        
        return Ok(new ResponseDto<List<GetAssessmentResponseAnalysisDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "The respective response analysis has been successfully retrieved.",
            Result = result
        });
    }
}