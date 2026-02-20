using CMSTrain.Application.DTOs.Answer;
using Microsoft.AspNetCore.Authorization;
using CMSTrain.Application.Common.Response;
using CMSTrain.Application.Interfaces.Services;

namespace CMSTrain.Controllers;

[Route("api/answer")]
public class AnswerController(IAnswerService answerService) : BaseController<AnswerController>
{
    [HttpPost("candidate")]
    public IActionResult UploadCandidateQuestionnaireAnswers(CandidateAnswerRequestDto candidateAnswers)
    {
        answerService.UploadCandidateQuestionnaireAnswers(candidateAnswers);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Your responses have been successfully saved, thank you :)",
            Result = true
        });
    }

    [AllowAnonymous]
    [HttpPost("subordinate")]
    public IActionResult UploadSubordinateQuestionnaireAnswers(SubordinateAnswerRequestDto subordinateAnswers)
    {
        answerService.UploadSubordinateQuestionnaireAnswers(subordinateAnswers);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Your responses have been successfully saved, thank you :)",
            Result = true
        });
    }
    
    [HttpGet("responses/{questionnaireId:guid}/{phase:int}")]
    public IActionResult GetResponseUserDetails(Guid questionnaireId, int phase)
    {
        var userResponses = answerService.GetResponseUserDetails(questionnaireId, phase);

        return Ok(new ResponseDto<List<GetResponseUserDetails>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Your responses have been successfully fetched.",
            Result = userResponses
        });
    }
    
    [HttpGet("responses/client/{questionnaireId:guid}/{phase:int}")]
    public IActionResult GetResponseUserDetailsForClient(Guid questionnaireId, int phase)
    {
        var userResponses = answerService.GetResponseUserDetailsForClient(questionnaireId, phase);

        return Ok(new ResponseDto<List<GetResponseUserDetails>>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Your responses have been successfully fetched.",
            Result = userResponses
        });
    }

    [HttpGet("{userResponseId:guid}")]
    public IActionResult GetQuestionAnswerDetails(Guid userResponseId)
    {
        var userResponses = answerService.GetQuestionAnswerDetails(userResponseId);

        return Ok(new ResponseDto<GetAnswerDetailsDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Your responses have been successfully fetched.",
            Result = userResponses
        });
    }
    
    [HttpGet("user-response/{userResponseId:guid}")]
    public IActionResult GetUserResponseDetails(Guid userResponseId)
    {
        var userResponses = answerService.GetUserResponseDetails(userResponseId);

        return Ok(new ResponseDto<GetUserResponseDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Your responses have been successfully fetched.",
            Result = userResponses
        });
    }
}
