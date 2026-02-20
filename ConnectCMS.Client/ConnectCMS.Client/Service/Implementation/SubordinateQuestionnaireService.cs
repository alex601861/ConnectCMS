using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Requests.Questionnaires;
using CMSTrain.Client.Models.Responses.Candidate;
using CMSTrain.Client.Models.Responses.Inspection;
using CMSTrain.Client.Models.Responses.Subordinate;
using CMSTrain.Client.Models.Responses.Training;
using CMSTrain.Client.Models.Responses.TrainingInspection;
using CMSTrain.Client.Service.Base;
using CMSTrain.Client.Service.Interface;

namespace CMSTrain.Client.Service.Implementation;

public class SubordinateQuestionnaireService(IBaseService baseService) : ISubordinateQuestionnaireService
{
    public async Task<ResponseDto<GetTrainingDto?>?> GetTrainingById(Guid trainingId)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString()
        };
        
        var response = await baseService.GetAsync<GetTrainingDto?>(ApiEndpoints.SubordinateQuestionnaire.GetTrainingById, pathParameter);

        return response;
    }

    public async Task<ResponseDto<GetInspectionDto?>?> GetInspectionById(Guid inspectionId)
    {
        var pathParameter = new List<string>
        {
            inspectionId.ToString()
        };
        
        var response = await baseService.GetAsync<GetInspectionDto?>(ApiEndpoints.SubordinateQuestionnaire.GetInspectionById, pathParameter);

        return response;
    }

    public async Task<ResponseDto<GetSubordinateDto?>?> GetSubordinateById(Guid subordinateId)
    {
        var pathParameter = new List<string>
        {
            subordinateId.ToString()
        };
        
        var response = await baseService.GetAsync<GetSubordinateDto?>(ApiEndpoints.SubordinateQuestionnaire.GetSubordinateById, pathParameter);

        return response;
    }

    public async Task<ResponseDto<GetCandidateDetailsDto?>?> GetCandidateBySubordinateId(Guid subordinateId)
    {
        var pathParameter = new List<string>
        {
            subordinateId.ToString()
        };
        
        var response = await baseService.GetAsync<GetCandidateDetailsDto?>(ApiEndpoints.SubordinateQuestionnaire.GetCandidateBySubordinateId, pathParameter);

        return response;
    }
    
    public async Task<ResponseDto<GetTrainingInspectionDetailsDto?>?> GetTrainingInspectionById(Guid trainingInspectionId)
    {
        var pathParameter = new List<string>
        {
            trainingInspectionId.ToString()
        };
        
        var response = await baseService.GetAsync<GetTrainingInspectionDetailsDto?>(ApiEndpoints.SubordinateQuestionnaire.GetTrainingInspectionById, pathParameter);

        return response;
    }

    public async Task<ResponseDto<GetCandidateQuestionnaireDto?>?> GetAllQuestionnairesForSubordinates(Guid questionnaireId, Guid subordinateId)
    {
        var pathParameter = new List<string>
        {
            questionnaireId.ToString(),
            subordinateId.ToString()
        };
        
        var response = await baseService.GetAsync<GetCandidateQuestionnaireDto?>(ApiEndpoints.SubordinateQuestionnaire.GetAllQuestionnairesForSubordinates, pathParameter);

        return response;
    }
}