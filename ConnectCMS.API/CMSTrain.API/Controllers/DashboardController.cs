using CMSTrain.Application.DTOs.Dashboard;
using CMSTrain.Application.Common.Response;
using CMSTrain.Application.Interfaces.Services;

namespace CMSTrain.Controllers;

[Route("api/dashboard")]
public class DashboardController(IDashboardService dashboardService) : BaseController<DashboardController>
{
    #region Admin
    [HttpGet("admin/dashboard-count/{period:int}")]
    public IActionResult GetAdminDashboardCount(int period)
    {
        var result = dashboardService.GetAdminDashboardCount(period);

        return Ok(new ResponseDto<GetAdminCountDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Admin dashboard count successfully fetched.",
            Result = result
        });
    }

    [HttpGet("admin/upcoming-trainings")]
    public IActionResult GetUpcomingTrainings()
    {
        var result = dashboardService.GetUpcomingTrainings();

        return Ok(new ResponseDto<List<GetUpcomingTrainingsDto>>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "The upcoming third training successfully fetched.",
            Result = result
        });
    }

    [HttpGet("admin/popular-trainings")]
    public IActionResult GetPopularTrainings()
    {
        var result = dashboardService.GetPopularTrainings();

        return Ok(new ResponseDto<List<GetPopularTrainingDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "The top third popular training successfully fetched.",
            Result = result
        });
    }

    [HttpGet("admin/training-format-count")]
    public IActionResult GetTotalTrainingFormats()
    {
        var result = dashboardService.GetTotalTrainingFormats();

        return Ok(new ResponseDto<List<GetTrainingFormatCountDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Total training formats successfully fetched.",
            Result = result
        });
    }

    [HttpGet("admin/training-request-summary/{year:int}")]
    public IActionResult TrainingRequestSummary(int year)
    {
        var result = dashboardService.GetTrainingRequestSummary(year);

        return Ok(new ResponseDto<GetTrainingRequestsSummaryDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Total candidate request summary successfully fetched.",
            Result = result
        });
    }
    #endregion 

    #region Trainer
    [HttpGet("trainer/dashboard-count/{period:int}")]
    public IActionResult GetTrainerDashboardCount(int period)
    {
        var result = dashboardService.GetTrainerDashboardCount(period);

        return Ok(new ResponseDto<GetDashboardCount>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Successfully retrieved trainer dashboard counts.",
            Result = result,
        });
    }

    [HttpGet("trainer/total-classes")]
    public IActionResult GetTotalClassesTrainer()
    {
        var result = dashboardService.GetTotalClassesForTrainer();

        return Ok(new ResponseDto<List<GetTotalClasses>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Successfully retrieved total classes for the trainer.",
            Result = result,
        });
    }

    [HttpGet("trainer/active-trainings")]
    public IActionResult GetActiveTrainings()
    {
        var result = dashboardService.GetAllActiveTrainings();

        return Ok(new ResponseDto<List<GetActiveTrainings>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Successfully retrieved all active trainings for the trainer.",
            Result = result,
        });
    }

    [HttpGet("trainer/upcoming-classes")]
    public IActionResult GetUpcomingClasses()
    {
        var result = dashboardService.GetUpcomingClasses();

        return Ok(new ResponseDto<List<GetClassDetails>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Successfully retrieved all upcoming classes info for the trainer.",
            Result = result,
        });
    }

    [HttpGet("trainer/class/{date}")]
    public IActionResult GetClassesDate(DateOnly date)
    {
        var result = dashboardService.GetClassesForDatesForTrainer(date);

        return Ok(new ResponseDto<List<GetClassesForDate>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Successfully retrieved all classes for the following date.",
            Result = result,
        });
    }

    [HttpGet("trainer/completed-classes")]
    public IActionResult GetCompletedClasses()
    {
        var result = dashboardService.GetCompletedClasses();

        return Ok(new ResponseDto<List<GetClassDetails>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Successfully retrieved all the completed classes.",
            Result = result,
        });
    }
    #endregion

    #region Candidates
    [HttpGet("candidate/training-progress/{period:int}")]
    public IActionResult GetTrainingProgress(int period)
    {
        var result = dashboardService.GetTrainingProgressesForCandidate(period);

        return Ok(new ResponseDto<GetCandidateTrainingProgressDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Successfully retrieved training progress.",
            Result = result,
        });
    }

    [HttpGet("candidate/assigned-trainings")]
    public IActionResult GetAssignedTrainings()
    {
        var result = dashboardService.GetAssignedTrainingsForCandidate();

        return Ok(new ResponseDto<List<GetAssignedTrainingDto>>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Successfully retrieved assigned trainings.",
            Result = result,
        });
    }

    [HttpGet("candidate/new-trainings")]
    public IActionResult GetNewTrainings()
    {
        var result = dashboardService.GetNewTrainingsForCandidate();

        return Ok(new ResponseDto<List<GetNewTrainingsDto>>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Successfully retrieved new trainings.",
            Result = result,
        });
    }
    
        [HttpGet("candidate/classes-for-date/{date}")]
    public IActionResult GetClassesForDatesForCandidates(DateOnly date)
    {
        var result = dashboardService.GetClassesForDatesForCandidates(date);

        return Ok(new ResponseDto<List<GetClassesForDate>>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Successfully retrieved classes for the specified date.",
            Result = result,
        });
    }

    [HttpGet("candidate/unanswered-questionnaires")]
    public IActionResult GetUnansweredQuestionnaireDetailsForCandidate()
    {
        var result = dashboardService.GetUnansweredQuestionnaireDetailsForCandidate();

        return Ok(new ResponseDto<List<GetQuestionnaireDto>>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Successfully retrieved unanswered questionnaires for the candidate.",
            Result = result,
        });
    }
    #endregion

    #region Client
    [HttpGet("client/training-progress/{period:int}")]
    public IActionResult GetTrainingProgressesForClient(int period)
    {
        var result = dashboardService.GetTrainingProgressesForClient(period);

        return Ok(new ResponseDto<GetClientTrainingProgressDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Successfully retrieved training progress for the client.",
            Result = result,
        });
    }

    [HttpGet("client/assigned-trainings")]
    public IActionResult GetAssignedTrainingsForClient()
    {
        var result = dashboardService.GetAssignedTrainingsForClient();

        return Ok(new ResponseDto<List<GetAssignedTrainingDto>>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Successfully retrieved assigned trainings for the client.",
            Result = result,
        });
    }

    [HttpGet("client/new-trainings")]
    public IActionResult GetNewTrainingsForClient()
    {
        var result = dashboardService.GetNewTrainingsForClient();

        return Ok(new ResponseDto<List<GetNewTrainingsDto>>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Successfully retrieved new trainings for the client.",
            Result = result,
        });
    }

    [HttpGet("client/classes-for-date/{date}")]
    public IActionResult GetClassesForDatesForClient(DateOnly date)
    {
        var result = dashboardService.GetClassesForDatesForClient(date);

        return Ok(new ResponseDto<List<GetClassesForDate>>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Successfully retrieved classes for the specified date for the client.",
            Result = result,
        });
    }

    [HttpGet("client/unanswered-questionnaires")]
    public IActionResult GetUnansweredQuestionnaireDetailsForClient()
    {
        var result = dashboardService.GetUnansweredQuestionnaireDetailsForClient();

        return Ok(new ResponseDto<List<GetQuestionnaireDto>>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Successfully retrieved unanswered questionnaires for the client.",
            Result = result,
        });
    }
    #endregion

    #region Generic APIs
    [HttpGet("training/classes/status")]
    public IActionResult GetAllClassesForUser()
    {
        var result = dashboardService.GetAllClassesForUser();

        return Ok(new ResponseDto<List<GetAllClassesDto>>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Successfully retrieved all classes for the user.",
            Result = result,
        });
    }
    #endregion
}