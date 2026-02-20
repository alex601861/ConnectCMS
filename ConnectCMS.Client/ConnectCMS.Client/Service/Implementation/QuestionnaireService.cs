using System.Text.Json;
using Microsoft.JSInterop;
using System.Net.Http.Headers;
using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Interface;
using CMSTrain.Client.Models.Responses.Answers;
using CMSTrain.Client.Models.Requests.Questionnaires;
using CMSTrain.Client.Models.Responses.Questionnaires;

namespace CMSTrain.Client.Service.Implementation;

public class QuestionnaireService(IBaseService baseService, IJSRuntime jsRuntime) : IQuestionnaireService
{
    public async Task<ResponseDto<GetQuestionnaireDetailsDto?>?> GetQuestionnaireModuleDetails(Guid questionnaireId)
    {
        var pathParameter = new List<string>()
        {
            questionnaireId.ToString()
        };
        
        var response = await baseService.GetAsync<GetQuestionnaireDetailsDto?>(ApiEndpoints.Questionnaire.GetQuestionnaireModuleDetails, pathParameter);

        return response;
    }
    
    public async Task<ResponseDto<GetQuestionnaireDto?>?> GetQuestionnaireDetails(Guid questionnaireId)
    {
        var pathParameter = new List<string>
        {
            questionnaireId.ToString()
        };
        
        var response = await baseService.GetAsync<GetQuestionnaireDto?>(ApiEndpoints.Questionnaire.GetQuestionnaireDetails, pathParameter);

        return response;
    }

    public async Task<ResponseDto<GetQuestionnaireDto?>?> GetAllQuestionnairesFromInspectionUpload(Guid inspectionId)
    {
        var pathParameter = new List<string>
        {
            inspectionId.ToString()
        };
        
        var response = await baseService.GetAsync<GetQuestionnaireDto?>(ApiEndpoints.Questionnaire.GetAllQuestionnairesFromInspectionUpload, pathParameter);

        return response;
    }

    public async Task<ResponseDto<GetQuestionnaireDto?>?> GetAllQuestionnairesForTrainingInspection(Guid trainingInspectionId)
    {
        var pathParameter = new List<string>
        {
            trainingInspectionId.ToString()
        };
        
        var response = await baseService.GetAsync<GetQuestionnaireDto?>(ApiEndpoints.Questionnaire.GetAllQuestionnairesForTrainingInspection, pathParameter);

        return response;
    }
    
    public async Task<ResponseDto<GetCandidateQuestionnaireDto?>?> GetAllQuestionnairesForCandidates(Guid questionnaireId)
    {
        var pathParameter = new List<string>()
        {
            questionnaireId.ToString()
        };
        
        var response = await baseService.GetAsync<GetCandidateQuestionnaireDto?>(ApiEndpoints.Questionnaire.GetAllQuestionnairesForCandidates, pathParameter);

        return response;
    }
    
    public async Task<ResponseDto<bool?>?> DownloadExcelFormat(Guid trainingId)
    {
        var pathParameter = new List<string>()
        {
            trainingId.ToString()
        };
        
        var result = await baseService.DownloadAsync(ApiEndpoints.Questionnaire.DownloadExcelFormat, pathParameter);

        if (result is not { content: not null, response: not null })
        {
            return new ResponseDto<bool?>()
            {
                Result = false,
                Message = "Questionnaire template could not be downloaded.",
                StatusCode = StatusCode.Status400BadRequest
            };
        }

        var response = result.response;
        
        var content = result.content;
            
        var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";

        await jsRuntime.InvokeVoidAsync("downloadFile", content, "Questionnaire Template", contentType);

        return new ResponseDto<bool?>()
        {
            Result = true,
            Message = "Successfully downloaded",
            StatusCode = StatusCode.Status200Ok
        };
    }
    
    public async Task<ResponseDto<bool?>?> UploadQuestionnaires(QuestionnaireExcelUploadDto questionnairesExcel)
    {
        var formData = new MultipartFormDataContent();

        formData.Add(new StringContent(questionnairesExcel.TrainingInspectionId.ToString()), "TrainingInspectionId");

        var questionnaireFileContent = new StreamContent(questionnairesExcel.File.OpenReadStream(long.MaxValue));
        
        questionnaireFileContent.Headers.ContentType = new MediaTypeHeaderValue(questionnairesExcel.File.ContentType);
        
        formData.Add(questionnaireFileContent, "File", questionnairesExcel.File.Name);
        
        var response = await baseService.UploadAsync<bool?>(ApiEndpoints.Questionnaire.UploadQuestionnairesViaExcel, Constants.UploadType.Post, formData);

        return response;
    }

    public async Task<ResponseDto<bool?>?> UploadQuestionnaires(QuestionnaireUploadDto questionnaires)
    {
        var jsonRequest = JsonSerializer.Serialize(questionnaires);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.PostAsync<bool?>(ApiEndpoints.Questionnaire.UploadQuestionnaires, content);
        
        return response;
    }
    
    public async Task<ResponseDto<bool?>?> DownloadQuestionnaireSheet(Guid questionnaireId)
    {
        var pathParameter = new List<string>()
        {
            questionnaireId.ToString()
        };
        
        var result = await baseService.DownloadAsync(ApiEndpoints.Questionnaire.DownloadQuestionnaireSheet, pathParameter);

        if (result is not { content: not null, response: not null })
        {
            return new ResponseDto<bool?>()
            {
                Result = false,
                Message = "Questionnaire sheet could not be downloaded.",
                StatusCode = StatusCode.Status400BadRequest
            };
        }

        var response = result.response;
        
        var content = result.content;
        
        var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";

        await jsRuntime.InvokeVoidAsync("downloadFile", content, $"Questionnaire Sheet - {questionnaireId.ToString()}", contentType);

        return new ResponseDto<bool?>()
        {
            Result = true,
            Message = "Questionnaire sheet successfully downloaded.",
            StatusCode = StatusCode.Status200Ok
        };
    }
    
    public async Task<ResponseDto<GetQuestionnaireValidityDto?>?> GetQuestionnaireValidity(Guid questionnaireId)
    {
        var pathParameter = new List<string>()
        {
            questionnaireId.ToString(),
        };
        
        var response = await baseService.GetAsync<GetQuestionnaireValidityDto?>(ApiEndpoints.Questionnaire.GetQuestionnaireValidity, pathParameter);

        return response;
    }

    public async Task<ResponseDto<GeneralQuestionnaireAnswerResponseDto?>?> GetGeneralQuestionnaireAnswerResponses(Guid questionnaireId, int phase)
    {
        var pathParameter = new List<string>
        {
            questionnaireId.ToString(),
            phase.ToString()
        };
        
        var response = await baseService.GetAsync<GeneralQuestionnaireAnswerResponseDto?>(ApiEndpoints.Questionnaire.GetGeneralQuestionnaireAnswerResponses, pathParameter);

        return response;
    }
    
    public async Task<ResponseDto<bool?>?> ExportQuestionnaireDetails(Guid questionnaireId, int phase)
    {
        var pathParameter = new List<string>
        {
            questionnaireId.ToString(),
            phase.ToString()
        };
        
        var result = await baseService.DownloadAsync(ApiEndpoints.Questionnaire.ExportQuestionnaireDetails, pathParameter);

        if (result is not { content: not null, response: not null })
        {
            return new ResponseDto<bool?>
            {
                Result = false,
                Message = "Excel File could not be downloaded",
                StatusCode = StatusCode.Status400BadRequest
            };
        }

        var response = result.response;
        
        var content = result.content;
            
        var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";

        var trainingQuestionnaireDetails = await GetTrainingQuestionnaireDetails(questionnaireId);
        
        await jsRuntime.InvokeVoidAsync("downloadFile", content, $"{trainingQuestionnaireDetails?.Result?.Training.Title} - {trainingQuestionnaireDetails?.Result?.Inspection.Name}", contentType);

        return new ResponseDto<bool?>()
        {
            Result = true,
            Message = "Excel File successfully downloaded.",
            StatusCode = StatusCode.Status200Ok
        };
    }

    public async Task<ResponseDto<GetTrainingQuestionnaireDto?>?> GetTrainingQuestionnaireDetails(Guid questionnaireId)
    {
        var pathParameter = new List<string>
        {
            questionnaireId.ToString()
        };
        
        var response = await baseService.GetAsync<GetTrainingQuestionnaireDto?>(ApiEndpoints.Questionnaire.GetTrainingQuestionnaireDetails, pathParameter);

        return response;
    }
    
    public async Task<ResponseDto<byte[]?>?> GenerateQuestionnaireAnswerUploadFormQrCode(Guid questionnaireId, string inspectionType)
    {
        var pathParameter = new List<string>()
        {
            questionnaireId.ToString(),
            inspectionType
        };
        
        var response = await baseService.GetAsync<byte[]?>(ApiEndpoints.Questionnaire.GenerateQuestionnaireAnswerUploadFormQrCode, pathParameter);

        return response;
    }
    
    public async Task<ResponseDto<bool?>?> DownloadQuestionnaireAnswerUploadFormQrCode(ResourceDownloadQrCodeDto resourceDownloadQrCode)
    {
        var pathParameter = new List<string>()
        {
            resourceDownloadQrCode.QuestionnaireId.ToString(),
        };
        
        var questionnaire =
            await baseService.GetAsync<GetTrainingQuestionnaireDto>(ApiEndpoints.Questionnaire.GetTrainingQuestionnaireDetails, pathParameter);

        var questionnaireDetails =
            $"QRCode_{questionnaire?.Result?.Training.Title} ({questionnaire?.Result?.Inspection.Name}).png";
        
        var jsonRequest = JsonSerializer.Serialize(resourceDownloadQrCode);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var result = await baseService.DownloadAsync(ApiEndpoints.Questionnaire.DownloadQuestionnaireAnswerUploadFormQrCode, content);

        if (result is not { content: not null, response: not null })
        {
            return new ResponseDto<bool?>()
            {
                Result = false,
                Message = "QR Code for the respective resource material could not be generated.",
                StatusCode = StatusCode.Status400BadRequest
            };
        }

        var response = result.response;
        
        var responseContent = result.content;
            
        var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";

        await jsRuntime.InvokeVoidAsync("downloadFile", responseContent, questionnaireDetails, contentType);

        return new ResponseDto<bool?>()
        {
            Result = true,
            Message = "QR Code for the respective resource material successfully generated.",
            StatusCode = StatusCode.Status200Ok
        };
    }
}