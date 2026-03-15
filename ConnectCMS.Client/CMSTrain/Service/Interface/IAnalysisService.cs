using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Dependency;
using CMSTrain.Client.Models.Responses.Analysis;
using UploadUserResponseAnalysisDto = CMSTrain.Client.Models.Requests.Analysis.UploadUserResponseAnalysisDto;

namespace CMSTrain.Client.Service.Interface;

public interface IAnalysisService : ITransientService
{
    Task<ResponseDto<bool?>?> UploadUserResponseAnalysis(UploadUserResponseAnalysisDto userResponseAnalysis);
    
    Task<ResponseDto<GetUserResponseAnalysisDto?>?> GetUserResponseAnalysisDetailsForFeedbacks(Guid userResponseId);

    Task<ResponseDto<List<GetAssessmentResponseAnalysisDto>?>?> GetUserResponseAnalysisDetailsForAssessments(Guid userResponseId);
    
    Task<ResponseDto<List<GetAssessmentResponseAnalysisDto>?>?> GetUserResponseAnalysisEvaluationDetailsForAssessments(Guid questionnaireId, Guid userResponseId, bool isSubordinateRequired, int phase);
}