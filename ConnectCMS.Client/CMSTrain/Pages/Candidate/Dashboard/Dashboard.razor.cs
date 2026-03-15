using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.Dashboard;
using CMSTrain.Client.Models.Responses.Identity;
using CMSTrain.Client.Service.Extensions;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CMSTrain.Client.Pages.Candidate.Dashboard;

public partial class Dashboard : ComponentBase
{
    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        await GetCandidateDashboardCount();
        await GetNewTrainings();
        await GetAssignTrainings();
        await OnDateChanged();
        await GetQuestionnaireProgress();
        await GetAllClasses();
        await GetUserName();
    }

    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.Dashboard;
    }
    #endregion
    
    #region Dashboard Time Period Count
    private int TimePeriod { get; set; } = Constants.TimePeriod.All;

    private async Task OnStatusFilter(int timePeriod)
    {
        TimePeriod = timePeriod; 

        await GetCandidateDashboardCount();
    }
    #endregion

    private GetTrainingProgressDto CandidateDashboardCount { get; set; } = new();

    private List<GetNewTrainingsDto> NewTrainings { get; set; }  = [];

    private List<GetAssignedTrainingDto> AssignedTraining { get; set; }  = [];

    private List<GetClassesForDate> ClassForDate { get; set; }  = [];

    private List<GetQuestionnaireDto> Questionnaire { get; set; }  = [];
    
    private List<GetAllClassesDto> AllClasses { get; set; } = new();

    private DateTime? DateRequest { get; set; }  = DateTime.Now;

    private string FormattedDate { get; set; }  = ExtensionMethods.GetDayWithSuffix(DateTime.Now.Day);
    
    private string FormattedMonth { get; set; }  = DateTime.Now.ToString("MMMM");

    private string Search { get; set; } = "";

    private async Task GetCandidateDashboardCount()
    {
        try
        {
            var response = await DashboardService.GetTrainingProgress(TimePeriod);

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            CandidateDashboardCount = response.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }  
    }

    private async Task GetAssignTrainings()
    {
        try
        {
            var response = await DashboardService.GetAssignedTrainings();

            if(response?.Result is null)
            {

                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            AssignedTraining = response.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    
    private async Task GetNewTrainings()
    {
        try
        {
            var response = await DashboardService.GetNewTrainings();

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            NewTrainings = response.Result;

            foreach (var training in NewTrainings)
            {
                if (!string.IsNullOrEmpty(training.ImageUrl))
                {
                    training.ImageUrl =
                        FileManager.FetchFileUrl(training.ImageUrl, Constants.FilePath.TrainingsImagesFilePath);
                }
            }
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }

    private async Task OnDateChanged()
    {
        var dateOnly = DateOnly.FromDateTime(DateRequest!.Value);
        
        await GetClassesForDatesForCandidates(dateOnly);
    }

    private async Task GetClassesForDatesForCandidates(DateOnly requestDate)
    {
        try
        {
            var response = await DashboardService.GetClassesForDatesForCandidates(requestDate);

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }
            
            FormattedDate = ExtensionMethods.GetDayWithSuffix(requestDate.Day);
            
            FormattedMonth = requestDate.ToString("MMMM");

            ClassForDate = response.Result;

            foreach (var @class in ClassForDate)
            {
                @class.ClassImage = !string.IsNullOrEmpty(@class.ClassImage)
                    ? FileManager.FetchFileUrl(@class.ClassImage, Constants.FilePath.ClassesImagesFilePath) 
                    : "images/dummy-img.png";
            }
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    
    private async Task GetAllClasses()
    {
        var response = await DashboardService.GetAllClassesForUser();

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        AllClasses = response.Result;
    }
    
    private string CheckDate(DateTime date)
    {
        foreach (var classItem in AllClasses)
        {
            var classDate = classItem.ClassDates.ToDateTimeFromDateOnlyString();
            var status = classItem.Status;

            if (classDate.Date == date.Date)
            {
                if (status == Constants.Schedule.ScheduledAction)
                {
                    return "mud-theme-info";
                }
                else if (status == Constants.Schedule.CompletedAction)
                {
                    return "mud-theme-success";
                }
            }
        }

        return string.Empty;
    }
    
    private async Task GetQuestionnaireProgress()
    {
        try
        {
            var response = await DashboardService.GetUnansweredQuestionnaireDetailsForCandidate();

            if(response?.Result is null)
            {

                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            Questionnaire = response.Result;

        }
        catch(Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }

    private void NavigateToClassDetails(Guid classId)
    {
        NavigationManager.NavigateTo($"/candidate/assigned-trainings/candidate-class-details/{classId}");
    }

    #region Assign Username
    private UserDetail UserDetail { get; set; } = new();

    private async Task GetUserName()
    {
        var response = await ProfileService.GetUserProfile();

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        UserDetail = response.Result;
    }

    #endregion
}