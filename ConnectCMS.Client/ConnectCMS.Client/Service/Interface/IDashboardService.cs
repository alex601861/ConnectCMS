using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Models.Responses.Dashboard;
using CMSTrain.Client.Service.Dependency;
using MudBlazor;

namespace CMSTrain.Client.Service.Interface;

public interface IDashboardService : ITransientService
{
    #region Trainer
    Task<ResponseDto<GetTrainerDashboardCount?>?> GetTrainerDashboardCount(int periodActio);
    
    Task<ResponseDto<List<GetTotalClasses>?>?> GetTotalClassesForTrainer();
    
    Task<ResponseDto<List<GetActiveTrainings>?>?> GetAllActiveTrainings(); 
    
    Task<ResponseDto<List<GetClassDetails>?>?> GetUpcomingClasses();
    
    Task<ResponseDto<List<GetClassesForDate>?>?> GetClassesForDates(DateOnly date);
    
    Task<ResponseDto<List<GetClassDetails>?>?> GetCompletedClasses();
    #endregion

    #region Admin
    Task<ResponseDto<GetAdminCountDto?>?> GetAdminDashboardCount(int periodAction);

    Task<ResponseDto<List<GetPopularTrainingDto>?>?> GetPopularTrainings();

    Task<ResponseDto<List<GetUpcomingTrainingDto>?>?> GetUpcomingTrainings();

    Task<ResponseDto<List<GetTrainingFormatCountDto>?>?> GetTotalTrainingFormats();

    Task<ResponseDto<GetTrainingRequestsSummaryDto?>?> GetTrainingRequestSummary(int year);
    #endregion
    
    #region Candidates
    Task<ResponseDto<GetTrainingProgressDto?>?> GetTrainingProgress(int timePeriod);

    Task<ResponseDto<List<GetAssignedTrainingDto>?>?> GetAssignedTrainings();

    Task<ResponseDto<List<GetNewTrainingsDto>?>?> GetNewTrainings();

    Task<ResponseDto<List<GetClassesForDate>?>?> GetClassesForDatesForCandidates(DateOnly date);

    Task<ResponseDto<List<GetQuestionnaireDto>?>?> GetUnansweredQuestionnaireDetailsForCandidate();

    #endregion

    #region Client
    Task<ResponseDto<GetTrainingProgressDto?>?> GetTrainingProgressesForClient(int periodAction);

    Task<ResponseDto<List<GetAssignedTrainingDto>?>?> GetAssignedTrainingsForClient();

    Task<ResponseDto<List<GetNewTrainingsDto>?>?> GetNewTrainingsForClient();

    Task<ResponseDto<List<GetClassesForDate>?>?> GetClassesForDatesForClient(DateOnly date);

    Task<ResponseDto<List<GetQuestionnaireDto>?>?> GetUnansweredQuestionnaireDetailsForClient();
    #endregion

    #region Generic

    Task<ResponseDto<List<GetAllClassesDto>?>?> GetAllClassesForUser();
    #endregion
}