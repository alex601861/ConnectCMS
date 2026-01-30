using CMSTrain.Application.DTOs.Analysis;
using CMSTrain.Application.Common.Service;

namespace CMSTrain.Application.Interfaces.Services;

public interface IAnalysisService : ITransientService
{
    void UploadUserResponseAnalysis(UploadUserResponseAnalysisDto userResponseAnalysis);

    GetUserResponseAnalysisDto GetUserResponseAnalysisDetailsForFeedbacks(Guid userResponseId);

    List<GetAssessmentResponseAnalysisDto> GetUserResponseAnalysisDetailsForAssessments(Guid userResponseId);

    List<GetAssessmentResponseAnalysisDto> GetUserResponseAnalysisEvaluationDetailsForAssessments(Guid questionnaireId, Guid userResponseId, bool isSubordinateRequired, int phase);
}