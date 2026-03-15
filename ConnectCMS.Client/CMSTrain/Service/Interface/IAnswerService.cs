using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Dependency;
using CMSTrain.Client.Models.Requests.Answers;
using CMSTrain.Client.Models.Responses.Answers;

namespace CMSTrain.Client.Service.Interface;

public interface IAnswerService : ITransientService
{
    Task<ResponseDto<bool?>?> UploadCandidateQuestionnaireAnswers(CandidateAnswerRequestDto candidateAnswers);

    Task<ResponseDto<bool?>?> UploadSubordinateQuestionnaireAnswers(SubordinateAnswerRequestDto subordinateAnswers);

    Task<ResponseDto<GetAnswerDetailsDto?>?> GetQuestionAnswerDetails(Guid userResponseId);

    Task<ResponseDto<List<GetResponseUserDetails>?>?> GetResponseUserDetails(Guid questionnaireId, int phase);

    Task<ResponseDto<List<GetResponseUserDetails>?>?> GetResponseUserDetailsForClient(Guid questionnaireId, int phase);

    Task<ResponseDto<GetUserResponseDto?>?> GetUserResponseDetails(Guid userResponseId);
}