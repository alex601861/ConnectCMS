using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Dependency;
using CMSTrain.Client.Models.Requests.PersonalityTest;
using CMSTrain.Client.Models.Responses.PersonalityTest;

namespace CMSTrain.Client.Service.Interface;

public interface IPersonalityTestService : ITransientService
{
    Task<ResponseDto<GetPersonalityTestQuestionnaireDto?>?> GetPersonalityTestQuestionnaires(Guid questionnaireId);

    Task<ResponseDto<GetPersonalityTestResponseDto?>?> GetPersonalityTestResponses(Guid userResponseId);

    Task<ResponseDto<bool?>?> UploadPersonalityTestAnswers(PersonalityTestRequestDto personalityTestAnswers);

    Task<ResponseDto<GetPersonalityTestAnalysisDto?>?> GetPersonalityTestAnalysis(Guid userResponseId);
}