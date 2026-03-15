using System.Text.Json;
using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Interface;
using CMSTrain.Client.Models.Requests.Answers;
using CMSTrain.Client.Models.Requests.Questionnaires;
using CMSTrain.Client.Models.Responses.Answers;

namespace CMSTrain.Client.Service.Implementation;

public class AnswerService(IBaseService baseService) : IAnswerService
{
    public async Task<ResponseDto<bool?>?> UploadCandidateQuestionnaireAnswers(CandidateAnswerRequestDto candidateAnswers)
    {
        var jsonRequest = JsonSerializer.Serialize(candidateAnswers);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

        var response = await baseService.PostAsync<bool?>(ApiEndpoints.Answer.UploadCandidateQuestionnaireAnswers, content);

        return response;
    }

    public async Task<ResponseDto<bool?>?> UploadSubordinateQuestionnaireAnswers(SubordinateAnswerRequestDto subordinateAnswers)
    {
        var jsonRequest = JsonSerializer.Serialize(subordinateAnswers);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

        var response = await baseService.PostAsync<bool?>(ApiEndpoints.Answer.UploadSubordinateQuestionnaireAnswers, content);

        return response;
    }
    
    public async Task<ResponseDto<GetAnswerDetailsDto?>?> GetQuestionAnswerDetails(Guid userResponseId)
    {
        var pathParameters = new List<string>()
        {
            userResponseId.ToString()
        };
        
        var response = await baseService.GetAsync<GetAnswerDetailsDto?>(endpoint: ApiEndpoints.Answer.GetQuestionAnswerDetails, path: pathParameters);

        return response;
    }

    public async Task<ResponseDto<List<GetResponseUserDetails>?>?> GetResponseUserDetails(Guid questionnaireId, int phase)
    {
        var pathParameters = new List<string>()
        {
            questionnaireId.ToString(),
            phase.ToString()
        };
        
        var response = await baseService.GetAsync<List<GetResponseUserDetails>?>(endpoint: ApiEndpoints.Answer.GetResponseUserDetails, path: pathParameters);

        return response;
    }
    
    public async Task<ResponseDto<List<GetResponseUserDetails>?>?> GetResponseUserDetailsForClient(Guid questionnaireId, int phase)
    {
        var pathParameters = new List<string>()
        {
            questionnaireId.ToString(),
            phase.ToString()
        };
        
        var response = await baseService.GetAsync<List<GetResponseUserDetails>?>(endpoint: ApiEndpoints.Answer.GetResponseUserDetailsForClient, path: pathParameters);

        return response;
    }

    public async Task<ResponseDto<GetUserResponseDto?>?> GetUserResponseDetails(Guid userResponseId)
    {
        var pathParameters = new List<string>()
        {
            userResponseId.ToString()
        };
        
        var response = await baseService.GetAsync<GetUserResponseDto?>(endpoint: ApiEndpoints.Answer.GetUserResponseDetails, path: pathParameters);

        return response;
    }
}