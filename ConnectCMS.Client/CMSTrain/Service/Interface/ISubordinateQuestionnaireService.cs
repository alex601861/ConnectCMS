using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Models.Requests.Questionnaires;
using CMSTrain.Client.Models.Responses.Candidate;
using CMSTrain.Client.Models.Responses.Inspection;
using CMSTrain.Client.Models.Responses.Subordinate;
using CMSTrain.Client.Models.Responses.Training;
using CMSTrain.Client.Models.Responses.TrainingInspection;
using CMSTrain.Client.Service.Dependency;

namespace CMSTrain.Client.Service.Interface;

public interface ISubordinateQuestionnaireService : ITransientService
{
    Task<ResponseDto<GetTrainingDto?>?> GetTrainingById(Guid trainingId);

    Task<ResponseDto<GetInspectionDto?>?> GetInspectionById(Guid inspectionId);

    Task<ResponseDto<GetSubordinateDto?>?> GetSubordinateById(Guid subordinateId);

    Task<ResponseDto<GetCandidateDetailsDto?>?> GetCandidateBySubordinateId(Guid subordinateId);

    Task<ResponseDto<GetTrainingInspectionDetailsDto?>?> GetTrainingInspectionById(Guid trainingInspectionId);

    Task<ResponseDto<GetCandidateQuestionnaireDto?>?> GetAllQuestionnairesForSubordinates(Guid questionnaireId, Guid subordinateId);
}