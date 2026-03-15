using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.Dashboard;
using CMSTrain.Client.Service.Base;
using CMSTrain.Client.Service.Interface;

namespace CMSTrain.Client.Service.Implementation;

public class DashboardService(IBaseService baseService) : IDashboardService
{
    #region Admin
    public async Task<ResponseDto<GetAdminCountDto?>?> GetAdminDashboardCount(int periodAction)
    {
        var pathParameter = new List<string>
        {
            periodAction.ToString()
        };
        
        var response = await baseService.GetAsync<GetAdminCountDto>(ApiEndpoints.Dashboard.GetAdminDashboardCount, pathParameter);

        return response;
    }

    public async Task<ResponseDto<List<GetPopularTrainingDto>?>?> GetPopularTrainings()
    {
        var response = await baseService.GetAsync<List<GetPopularTrainingDto>?>(ApiEndpoints.Dashboard.GetPopularTrainings);

        return response;
    }

    public async Task<ResponseDto<List<GetUpcomingTrainingDto>?>?> GetUpcomingTrainings()
    {
        var response = await baseService.GetAsync<List<GetUpcomingTrainingDto>>(ApiEndpoints.Dashboard.GetUpcomingTrainings);

        return response;    
    }

    public async Task<ResponseDto<List<GetTrainingFormatCountDto>?>?> GetTotalTrainingFormats()
    {
        var response = await baseService.GetAsync<List<GetTrainingFormatCountDto>?>(ApiEndpoints.Dashboard.GetTotalTrainingFormats);

        return response;
    }

    public async Task<ResponseDto<GetTrainingRequestsSummaryDto?>?> GetTrainingRequestSummary(int year)
    {
        var pathParameter = new List<string>
        {
            year.ToString()
        };
        
        var response = await baseService.GetAsync<GetTrainingRequestsSummaryDto?>(ApiEndpoints.Dashboard.GetTrainingRequestSummary, pathParameter);

        return response;
    }
    #endregion

    #region Trainer
    public async Task<ResponseDto<GetTrainerDashboardCount?>?> GetTrainerDashboardCount(int periodAction)
    {
        var pathParameter = new List<string>
        {
            periodAction.ToString()
        };
        
        var response = await baseService.GetAsync<GetTrainerDashboardCount>(ApiEndpoints.Dashboard.GetTrainerDashboardCount, pathParameter);

        return response;
    }

    public async Task<ResponseDto<List<GetTotalClasses>?>?> GetTotalClassesForTrainer()
    {
        var response = await baseService.GetAsync<List<GetTotalClasses>?>(ApiEndpoints.Dashboard.GetTrainerTotalClasses);

        return response;
    }

    public async Task<ResponseDto<List<GetActiveTrainings>?>?> GetAllActiveTrainings()
    {
        var response = await baseService.GetAsync<List<GetActiveTrainings>?>(ApiEndpoints.Dashboard.GetTrainerActiveTrainings);

        return response;
    }

    public async Task<ResponseDto<List<GetClassDetails>?>?> GetUpcomingClasses()
    {
        var response = await baseService.GetAsync<List<GetClassDetails>?>(ApiEndpoints.Dashboard.GetTrainerUpcomingClasses);

        return response;
    }

    public async Task<ResponseDto<List<GetClassesForDate>?>?> GetClassesForDates(DateOnly date)
    {
        var pathParameter = new List<string>
        {
            date.ToString("MM.dd.yyyy")
        };

        var response = await baseService.GetAsync<List<GetClassesForDate>?>(ApiEndpoints.Dashboard.GetTrainerClassesByDate, pathParameter);

        return response;
    }

    public async Task<ResponseDto<List<GetClassDetails>?>?> GetCompletedClasses()
    {
        var response = await baseService.GetAsync<List<GetClassDetails>?>(ApiEndpoints.Dashboard.GetTrainerCompletedClasses);

        return response;
    }
    #endregion

    #region Candidates
    public async Task<ResponseDto<GetTrainingProgressDto?>?> GetTrainingProgress(int periodAction)
    {
        var pathParameter = new List<string>
        {
            periodAction.ToString()
        };
        
        var response = await baseService.GetAsync<GetTrainingProgressDto>(ApiEndpoints.Dashboard.GetTrainingProgress, pathParameter);
        
        return response;
    }

    public async Task<ResponseDto<List<GetAssignedTrainingDto>?>?> GetAssignedTrainings()
    {
        var response = await baseService.GetAsync<List<GetAssignedTrainingDto>?>(ApiEndpoints.Dashboard.GetAssignedTrainings);
       
        return response;
    }

    public async Task<ResponseDto<List<GetNewTrainingsDto>?>?> GetNewTrainings()
    {
        var response = await baseService.GetAsync<List<GetNewTrainingsDto>?>(ApiEndpoints.Dashboard.GetNewTrainings);

        return response;
    }

    public async Task<ResponseDto<List<GetClassesForDate>?>?> GetClassesForDatesForCandidates(DateOnly date)
    {
        var pathParameter = new List<string>
        {
            date.ToString("MM.dd.yyyy")
        };

        var response = await baseService.GetAsync<List<GetClassesForDate>?>(ApiEndpoints.Dashboard.GetClassesForDatesForCandidates, pathParameter);

        return response;    
    }

    public async Task<ResponseDto<List<GetQuestionnaireDto>?>?> GetUnansweredQuestionnaireDetailsForCandidate()
    {
        var response = await baseService.GetAsync<List<GetQuestionnaireDto>?>(ApiEndpoints.Dashboard.GetUnansweredQuestionnaireDetailsForCandidate);

        return response;
    }
    #endregion
    
    #region Client
    public async Task<ResponseDto<GetTrainingProgressDto?>?> GetTrainingProgressesForClient(int periodAction)
    {
        var pathParameter = new List<string>
        {
            periodAction.ToString()
        };
        
        var response = await baseService.GetAsync<GetTrainingProgressDto>(ApiEndpoints.Dashboard.GetTrainingProgressesForClient, pathParameter);

        return response;
    }

    public async Task<ResponseDto<List<GetAssignedTrainingDto>?>?> GetAssignedTrainingsForClient()
    {
        var response = await baseService.GetAsync<List<GetAssignedTrainingDto>?>(ApiEndpoints.Dashboard.GetAssignedTrainingsForClient);

        return response;
    }

    public async Task<ResponseDto<List<GetNewTrainingsDto>?>?> GetNewTrainingsForClient()
    {
        var response = await baseService.GetAsync<List<GetNewTrainingsDto>?>(ApiEndpoints.Dashboard.GetNewTrainingsForClient);

        return response;
    }

    public async Task<ResponseDto<List<GetClassesForDate>?>?> GetClassesForDatesForClient(DateOnly date)
    {
        var pathParameter = new List<string>
        {
            date.ToString("MM.dd.yyyy")
        };

        var response = await baseService.GetAsync<List<GetClassesForDate>?>(ApiEndpoints.Dashboard.GetClassesForDatesForClient, pathParameter);

        return response;
    }

    public async Task<ResponseDto<List<GetQuestionnaireDto>?>?> GetUnansweredQuestionnaireDetailsForClient()
    {
        var response = await baseService.GetAsync<List<GetQuestionnaireDto>?>(ApiEndpoints.Dashboard.GetUnansweredQuestionnaireDetailsForClient);

        return response;
    }

    public async Task<ResponseDto<List<GetAllClassesDto>?>?> GetAllClassesForUser()
    {
        var response = await baseService.GetAsync<List<GetAllClassesDto>?>(ApiEndpoints.Dashboard.GetAllClasses);

        return response;
    }
    #endregion
}