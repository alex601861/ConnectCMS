using CMSTrain.Application.DTOs.Answer;
using CMSTrain.Application.Common.Response;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.DTOs.Questionnaires;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;

namespace CMSTrain.Controllers;

[Route("api/questionnaire")]
public class QuestionnaireController(IQuestionnaireService questionnaireService, IAnswerService answerService) : BaseController<QuestionnaireController>
{
    [HttpGet("details/{questionnaireId:guid}")]
    public IActionResult GetQuestionnaireModuleDetails(Guid questionnaireId)
    {
        var result = questionnaireService.GetQuestionnaireModuleDetails(questionnaireId);

        return Ok(new ResponseDto<GetQuestionnaireDetailsDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Available questions are successfully fetched.",
            Result = result
        });
    }
    
    [HttpGet("{questionnaireId:guid}")]
    public IActionResult GetQuestionnaireDetails(Guid questionnaireId)
    {
        var result = questionnaireService.GetQuestionnaireDetails(questionnaireId);

        return Ok(new ResponseDto<GetQuestionnaireDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Available questions are successfully fetched.",
            Result = result
        });
    }
    
    [HttpGet("inspection/details/{trainingInspectionId:guid}")]
    public IActionResult GetAllQuestionnairesForTrainingInspection(Guid trainingInspectionId)
    {
        var result = questionnaireService.GetAllQuestionnairesForTrainingInspection(trainingInspectionId);

        return Ok(new ResponseDto<GetQuestionnaireDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Available questions are successfully fetched.",
            Result = result
        });
    }
    
    [HttpGet("inspection/{inspectionId:guid}")]
    public IActionResult GetAllQuestionnairesFromInspectionUpload(Guid inspectionId)
    {
        var questionnaire = questionnaireService.GetAllQuestionnairesFromInspectionUpload(inspectionId);

        return Ok(new ResponseDto<GetQuestionnaireDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Questionnaire retrieved successfully.",
            Result = questionnaire
        });
    }
    
    [HttpGet("candidate/{questionnaireId:guid}")]
    public IActionResult GetAllQuestionnairesForCandidates(Guid questionnaireId)
    {
        var result = questionnaireService.GetAllQuestionnairesForCandidate(questionnaireId);

        return Ok(new ResponseDto<GetCandidateQuestionnaireDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Available questions are successfully fetched.",
            Result = result
        });
    }
    
    [HttpPost]
    public IActionResult UploadQuestionnaires(QuestionnaireUploadDto questionnaires)
    {
        questionnaireService.UploadQuestionnaires(questionnaires);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Questionnaires successfully uploaded.",
            Result = true
        });
    }
    
    [HttpGet("validity/{questionnaireId:guid}")]
    public IActionResult GetQuestionnaireValidity(Guid questionnaireId)
    {
        var result = questionnaireService.GetQuestionnaireValidity(questionnaireId);

        return Ok(new ResponseDto<GetQuestionnaireValidityDto>()
        {
            Result = result,
            StatusCode = (int)HttpStatusCode.OK,
            Message = "The validity of the questionnaire has been successfully retrieved."
        });
    }

    [HttpGet("stats/{questionnaireId:guid}/{phase:int}")]
    public IActionResult GetGeneralQuestionnaireAnswerResponses(Guid questionnaireId, int phase)
    {
        var result = answerService.GetGeneralQuestionnaireAnswerResponses(questionnaireId, phase);

        return Ok(new ResponseDto<GeneralQuestionnaireAnswerResponseDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Available questions are successfully fetched.",
            Result = result
        });
    }
    
    [HttpGet("report/{questionnaireId:guid}/{phase:int}")]
    public IActionResult ExportQuestionnaireDetails(Guid questionnaireId, int phase)
    {
        var result = questionnaireService.ExportQuestionnaireDetails(questionnaireId, phase);
        
        var trainingQuestionnaire = questionnaireService.GetTrainingQuestionnaireDetails(questionnaireId);

        return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{trainingQuestionnaire.Training.Title} - {trainingQuestionnaire.Inspection.Name}");
    }
    
    [HttpGet("training/{questionnaireId:guid}")]
    public IActionResult GetTrainingQuestionnaireDetails(Guid questionnaireId)
    {
        var result= questionnaireService.GetTrainingQuestionnaireDetails(questionnaireId);

        return Ok(new ResponseDto<GetTrainingQuestionnaireDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Training Questionnaire Details Successfully fetched.",
            Result = result
        });
    }
    
    [HttpGet("qr-code/{questionnaireId:guid}/{inspectionType}")]
    public IActionResult GenerateQuestionnaireAnswerUploadFormQrCode(Guid questionnaireId, string inspectionType)
    {
        var result = questionnaireService.GenerateQuestionnaireAnswerUploadFormQrCode(questionnaireId, inspectionType);

        return Ok(new ResponseDto<byte[]>()
        {
            Result = result,
            StatusCode = (int)HttpStatusCode.OK,
            Message = "The QR Code of the respective questionnaire has been successfully generated."
        });
    }
    
    [HttpPost("qr-code")]
    public async Task<IActionResult> GenerateQuestionnaireAnswerUploadFormQrCode(QuestionnaireDownloadQrCodeDto questionnaireDownloadQrCode)
    {
        var qrCodeBytes = questionnaireService.GenerateQuestionnaireAnswerUploadFormQrCode(questionnaireDownloadQrCode.QuestionnaireId, questionnaireDownloadQrCode.InspectionType);

        var questionnaire = questionnaireService.GetTrainingQuestionnaireDetails(questionnaireDownloadQrCode.QuestionnaireId);
        
        using var image = Image.Load(qrCodeBytes);
        
        using var ms = new MemoryStream();
        
        await image.SaveAsync(ms, new PngEncoder());
        
        ms.Position = 0;

        return File(ms.ToArray(), "image/png", $"QRCode_{questionnaire.Training.Title} ({questionnaire.Inspection.Name}).png");
    }
}
