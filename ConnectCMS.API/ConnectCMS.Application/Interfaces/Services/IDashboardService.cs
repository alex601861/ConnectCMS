using CMSTrain.Application.Common.Service;
using CMSTrain.Application.DTOs.Dashboard;

namespace CMSTrain.Application.Interfaces.Services;

public interface IDashboardService : ITransientService
{
    #region Admin
    GetAdminCountDto GetAdminDashboardCount(int period);

    List<GetPopularTrainingDto> GetPopularTrainings();

    List<GetUpcomingTrainingsDto> GetUpcomingTrainings();

    List<GetTrainingFormatCountDto> GetTotalTrainingFormats();

    GetTrainingRequestsSummaryDto GetTrainingRequestSummary(int year);
    #endregion

    #region Trainers
    GetDashboardCount GetTrainerDashboardCount(int period);
    
    List<GetTotalClasses> GetTotalClassesForTrainer();

    List<GetActiveTrainings> GetAllActiveTrainings();

    List<GetClassDetails> GetUpcomingClasses();
    
    List<GetClassesForDate> GetClassesForDatesForTrainer(DateOnly date);
    
    List<GetClassDetails> GetCompletedClasses();
    #endregion

    #region Candidates
    GetCandidateTrainingProgressDto GetTrainingProgressesForCandidate(int period);

    List<GetAssignedTrainingDto> GetAssignedTrainingsForCandidate();

    List<GetNewTrainingsDto> GetNewTrainingsForCandidate();
    
    List<GetClassesForDate> GetClassesForDatesForCandidates(DateOnly date);

    List<GetQuestionnaireDto> GetUnansweredQuestionnaireDetailsForCandidate();
    #endregion
    
    #region Clients
    GetClientTrainingProgressDto GetTrainingProgressesForClient(int period);

    List<GetAssignedTrainingDto> GetAssignedTrainingsForClient();

    List<GetNewTrainingsDto> GetNewTrainingsForClient();
    
    List<GetClassesForDate> GetClassesForDatesForClient(DateOnly date);

    List<GetQuestionnaireDto> GetUnansweredQuestionnaireDetailsForClient();
    #endregion

    #region Generic
    List<GetAllClassesDto> GetAllClassesForUser();
    #endregion
}