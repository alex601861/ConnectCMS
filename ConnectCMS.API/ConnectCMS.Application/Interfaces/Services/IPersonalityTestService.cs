using CMSTrain.Application.Common.Service;
using CMSTrain.Application.DTOs.PersonalityTest;

namespace CMSTrain.Application.Interfaces.Services;

public interface IPersonalityTestService : ITransientService
{
    GetPersonalityTestQuestionnaireDto GetPersonalityTestQuestionnaires(Guid questionnaireId, bool isRandomizedDataRequired);
    
    GetPersonalityTestResponseDto GetPersonalityTestResponses(Guid userResponseId);
    
    void UploadPersonalityTestAnswers(PersonalityTestRequestDto personalityTestAnswers);
    
    GetPersonalityTestAnalysisDto GetPersonalityTestAnalysis(Guid userResponseId);
}