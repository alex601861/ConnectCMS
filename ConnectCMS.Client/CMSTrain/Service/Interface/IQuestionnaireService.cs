using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Dependency;
using CMSTrain.Client.Models.Responses.Answers;
using CMSTrain.Client.Models.Requests.Questionnaires;
using CMSTrain.Client.Models.Responses.Questionnaires;

namespace CMSTrain.Client.Service.Interface;

public interface IQuestionnaireService : ITransientService
{
    Task<ResponseDto<GetQuestionnaireDetailsDto?>?> GetQuestionnaireModuleDetails(Guid questionnaireId);

    Task<ResponseDto<GetQuestionnaireDto?>?> GetQuestionnaireDetails(Guid questionnaireId);
    
    Task<ResponseDto<GetQuestionnaireDto?>?> GetAllQuestionnairesFromInspectionUpload(Guid inspectionId);

    Task<ResponseDto<GetQuestionnaireDto?>?> GetAllQuestionnairesForTrainingInspection(Guid trainingInspectionId);
    
    Task<ResponseDto<GetCandidateQuestionnaireDto?>?> GetAllQuestionnairesForCandidates(Guid questionnaireId);
    
    Task<ResponseDto<bool?>?> DownloadExcelFormat(Guid trainingId);
    
    Task<ResponseDto<bool?>?> UploadQuestionnaires(QuestionnaireExcelUploadDto questionnairesExcel);

    Task<ResponseDto<bool?>?> UploadQuestionnaires(QuestionnaireUploadDto questionnaires);

    Task<ResponseDto<bool?>?> DownloadQuestionnaireSheet(Guid questionnaireId);
    
    Task<ResponseDto<GetQuestionnaireValidityDto?>?> GetQuestionnaireValidity(Guid questionnaireId);

    Task<ResponseDto<GeneralQuestionnaireAnswerResponseDto?>?> GetGeneralQuestionnaireAnswerResponses(Guid questionnaireId, int phase);
    
    Task<ResponseDto<bool?>?> ExportQuestionnaireDetails(Guid questionnaireId, int phase);

    Task<ResponseDto<GetTrainingQuestionnaireDto?>?> GetTrainingQuestionnaireDetails(Guid questionnaireId);
    
    Task<ResponseDto<byte[]?>?> GenerateQuestionnaireAnswerUploadFormQrCode(Guid questionnaireId, string inspectionType);

    Task<ResponseDto<bool?>?> DownloadQuestionnaireAnswerUploadFormQrCode(ResourceDownloadQrCodeDto resourceDownloadQrCode);
}