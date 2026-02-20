using CMSTrain.Application.Common.Response;
using CMSTrain.Application.DTOs.PersonalityTest;
using CMSTrain.Application.Interfaces.Services;

namespace CMSTrain.Controllers;

[Route("api/personality-test")]
public class PersonalityTestController(IPersonalityTestService personalityTestService) : BaseController<PersonalityTestController>
{
    [HttpGet("questionnaire/{questionnaireId:guid}")]
    public IActionResult GetPersonalityTestQuestionnaires(Guid questionnaireId)
    {
        var result = personalityTestService.GetPersonalityTestQuestionnaires(questionnaireId, true);

        return Ok(new ResponseDto<GetPersonalityTestQuestionnaireDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Available questions are successfully fetched.",
            Result = result
        });
    }
    
    [HttpGet("response/{userResponseId:guid}")]
    public IActionResult GetPersonalityTestResponses(Guid userResponseId)
    {
        var result = personalityTestService.GetPersonalityTestResponses(userResponseId);

        return Ok(new ResponseDto<GetPersonalityTestResponseDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Available questions are successfully fetched.",
            Result = result
        });
    }
    
    [HttpPost]
    public IActionResult UploadPersonalityTestAnswers(PersonalityTestRequestDto personalityTestAnswers)
    {
        personalityTestService.UploadPersonalityTestAnswers(personalityTestAnswers);
        
        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Your responses have been successfully saved, thank you :)",
            Result = true
        });
    }
    
    [HttpGet("analysis/{userResponseId:guid}")]
    public IActionResult GetPersonalityTestAnalysis(Guid userResponseId)
    {
        var result = personalityTestService.GetPersonalityTestAnalysis(userResponseId);

        return Ok(new ResponseDto<GetPersonalityTestAnalysisDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Available questions are successfully fetched.",
            Result = result
        });
    }
}