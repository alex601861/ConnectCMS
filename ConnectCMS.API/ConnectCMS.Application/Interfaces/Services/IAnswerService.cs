using CMSTrain.Application.DTOs.Answer;
using CMSTrain.Application.Common.Service;

namespace CMSTrain.Application.Interfaces.Services;

public interface IAnswerService : ITransientService
{
    void UploadCandidateQuestionnaireAnswers(CandidateAnswerRequestDto candidateAnswers);

    void UploadSubordinateQuestionnaireAnswers(SubordinateAnswerRequestDto subordinateAnswers);
    
    List<GetResponseUserDetails> GetResponseUserDetails(Guid questionnaireId, int phase);

    List<GetResponseUserDetails> GetResponseUserDetailsForClient(Guid questionnaireId, int phase);

    GetAnswerDetailsDto GetQuestionAnswerDetails(Guid userResponseId);
    
    GetUserResponseDto GetUserResponseDetails(Guid userResponseId);

    GeneralQuestionnaireAnswerResponseDto GetGeneralQuestionnaireAnswerResponses(Guid questionnaireId, int phase);

    int GetQuestionnaireAnswerResponseCount(GeneralQuestionAnswerResponseDto generalQuestionAnswerResponse);
}
