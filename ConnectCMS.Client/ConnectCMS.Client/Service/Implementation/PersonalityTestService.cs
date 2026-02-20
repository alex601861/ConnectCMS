using System.Text.Json;
using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Interface;
using CMSTrain.Client.Models.Requests.PersonalityTest;
using CMSTrain.Client.Models.Responses.PersonalityTest;

namespace CMSTrain.Client.Service.Implementation;

public class PersonalityTestService(IBaseService baseService) : IPersonalityTestService
{
    public async Task<ResponseDto<GetPersonalityTestQuestionnaireDto?>?> GetPersonalityTestQuestionnaires(Guid questionnaireId)
    {
        var pathParameter = new List<string>
        {
            questionnaireId.ToString()
        };
        
        var response = await baseService.GetAsync<GetPersonalityTestQuestionnaireDto?>(ApiEndpoints.PersonalityTest.GetPersonalityTestQuestionnaires, pathParameter);

        return response;
    }

    public async Task<ResponseDto<GetPersonalityTestResponseDto?>?> GetPersonalityTestResponses(Guid userResponseId)
    {
        var pathParameter = new List<string>
        {
            userResponseId.ToString()
        };
        
        var response = await baseService.GetAsync<GetPersonalityTestResponseDto?>(ApiEndpoints.PersonalityTest.GetPersonalityTestResponses, pathParameter);

        return response;
    }

    public async Task<ResponseDto<bool?>?> UploadPersonalityTestAnswers(PersonalityTestRequestDto personalityTestAnswers)
    {
        var jsonRequest = JsonSerializer.Serialize(personalityTestAnswers);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

        var response = await baseService.PostAsync<bool?>(ApiEndpoints.PersonalityTest.UploadPersonalityTestAnswers, content);

        return response;
    }

    public async Task<ResponseDto<GetPersonalityTestAnalysisDto?>?> GetPersonalityTestAnalysis(Guid userResponseId)
    {
        var pathParameter = new List<string>
        {
            userResponseId.ToString()
        };
        
        var response = await baseService.GetAsync<GetPersonalityTestAnalysisDto?>(ApiEndpoints.PersonalityTest.GetPersonalityTestAnalysis, pathParameter);

        return response;
    }
}