using CMSTrain.Application.Common.Service;
using CMSTrain.Application.DTOs.Questionnaires;

namespace CMSTrain.Application.Interfaces.Services;

public interface IQuestionnaireService : ITransientService
{
    GetQuestionnaireDetailsDto GetQuestionnaireModuleDetails(Guid questionnaireId);
    
    GetQuestionnaireDto GetQuestionnaireDetails(Guid questionnaireId);
    
    GetQuestionnaireDto GetAllQuestionnairesForTrainingInspection(Guid trainingInspectionId);

    GetQuestionnaireDto GetAllQuestionnairesFromInspectionUpload(Guid inspectionId);

    GetCandidateQuestionnaireDto GetAllQuestionnairesForCandidate(Guid questionnaireId);

    GetCandidateQuestionnaireDto GetAllQuestionnairesForSubordinates(Guid questionnaireId, Guid subordinateId);

    void UploadQuestionnaires(QuestionnaireUploadDto questionnaire);
    
    GetQuestionnaireValidityDto GetQuestionnaireValidity(Guid questionnaireId);
    
    byte[] ExportQuestionnaireDetails(Guid questionnaireId, int phase);

    GetTrainingQuestionnaireDto GetTrainingQuestionnaireDetails(Guid questionnaireId);
    
    byte[] GenerateQuestionnaireAnswerUploadFormQrCode(Guid questionnaireId, string inspectionType);
}
