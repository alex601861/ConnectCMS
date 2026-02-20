using System.Text.Json;
using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Interface;
using CMSTrain.Client.Models.Requests.Analysis;
using CMSTrain.Client.Models.Responses.Analysis;

namespace CMSTrain.Client.Service.Implementation;

public class AnalysisService(IBaseService baseService) : IAnalysisService
{
    public async Task<ResponseDto<bool?>?> UploadUserResponseAnalysis(UploadUserResponseAnalysisDto userResponseAnalysis)
    {
        var jsonRequest = JsonSerializer.Serialize(userResponseAnalysis);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

        var response = await baseService.PostAsync<bool?>(ApiEndpoints.Analysis.UploadUserResponseAnalysis, content);

        return response;
    }

    public async Task<ResponseDto<GetUserResponseAnalysisDto?>?> GetUserResponseAnalysisDetailsForFeedbacks(Guid userResponseId)
    {
        var pathParameter = new List<string>
        {
            userResponseId.ToString()
        };
        
        var response = await baseService.GetAsync<GetUserResponseAnalysisDto?>(ApiEndpoints.Analysis.GetUserResponseAnalysisDetailsForFeedbacks, pathParameter);

        return response;
    }

    public async Task<ResponseDto<List<GetAssessmentResponseAnalysisDto>?>?> GetUserResponseAnalysisDetailsForAssessments(Guid userResponseId)
    {
        var pathParameter = new List<string>
        {
            userResponseId.ToString()
        };
        
        var response = await baseService.GetAsync<List<GetAssessmentResponseAnalysisDto>?>(ApiEndpoints.Analysis.GetUserResponseAnalysisDetailsForAssessments, pathParameter);

        return response;
    }

    public async Task<ResponseDto<List<GetAssessmentResponseAnalysisDto>?>?> GetUserResponseAnalysisEvaluationDetailsForAssessments(Guid questionnaireId, Guid userResponseId, bool isSubordinateRequired, int phase)
    {
        var pathParameter = new List<string>
        {
            questionnaireId.ToString(),
            userResponseId.ToString(),
            isSubordinateRequired.ToString(),
            phase.ToString()
        };
        
        var response = await baseService.GetAsync<List<GetAssessmentResponseAnalysisDto>?>(ApiEndpoints.Analysis.GetUserResponseAnalysisEvaluationDetailsForAssessments, pathParameter);

        return response;
    }
}