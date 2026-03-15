using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.Dashboard;
using CMSTrain.Client.Models.Responses.Identity;
using CMSTrain.Client.Service.Extensions;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CMSTrain.Client.Pages.Client.Dashboard;

public partial class Dashboard : ComponentBase
{
    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        await GetClientDashboardCount();
        await GetAssignedTrainingsForClient();
        await GetNewTrainingsForClient();
        await GetUnansweredQuestionnaireDetailsForClient();
        await OnDateChanged();
        await GetAllClasses();
        await GetUserName();
    }
    
    private string Search { get; set; } = "";

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

        await GetClientDashboardCount();
    }
    #endregion

    #region Client Admin
    private GetTrainingProgressDto GetTrainingProgress { get; set; } = new();

    private List<GetAssignedTrainingDto> AssignedTraining { get; set; } = new();

    private List<GetNewTrainingsDto> NewTrainings { get; set; } = new();

    private List<GetClassesForDate> ClassesForDate {  get; set; } = new(); 

    private List<GetQuestionnaireDto> Questionnaire { get; set; } = new();
    
    private List<GetAllClassesDto> AllClasses { get; set; } = new();

    private DateTime? DateRequest { get; set; } = DateTime.Now;
    
    private string FormattedDate { get; set; }  = ExtensionMethods.GetDayWithSuffix(DateTime.Now.Day);
    
    private string FormattedMonth { get; set; }  = DateTime.Now.ToString("MMMM");
    
    private async Task GetClientDashboardCount()
    {
        try
        {
            var response = await DashboardService.GetTrainingProgressesForClient(TimePeriod);

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            GetTrainingProgress = response.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }

    private async Task GetAssignedTrainingsForClient()
    {
        try
        {
            var response = await DashboardService.GetAssignedTrainingsForClient();

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            AssignedTraining = response.Result;

        }
        catch(Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }

    private async Task GetNewTrainingsForClient()
    {
        try
        {
            var response = await DashboardService.GetNewTrainingsForClient();

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
        catch(Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }

    private async Task OnDateChanged()
    {
        var dateOnly = DateOnly.FromDateTime(DateRequest!.Value);
        
        await GetClassesForDatesForClient(dateOnly);
    }

    private async Task GetClassesForDatesForClient(DateOnly requestDate)
    {
        try
        {
            var response = await DashboardService.GetClassesForDatesForClient(requestDate);

            if (response?.Result is null)
            {

                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            ClassesForDate = response.Result;

            foreach (var classes in ClassesForDate)
            {
                classes.ClassImage = !string.IsNullOrEmpty(classes.ClassImage) 
                    ? FileManager.FetchFileUrl(classes.ClassImage, Constants.FilePath.ClassesImagesFilePath) 
                    : "images/dummy-img.png";
            }
            
            FormattedDate = ExtensionMethods.GetDayWithSuffix(requestDate.Day);
            
            FormattedMonth = requestDate.ToString("MMMM");
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

    private async Task GetUnansweredQuestionnaireDetailsForClient()
    {
        try
        {
            var response = await DashboardService.GetUnansweredQuestionnaireDetailsForClient();

            if (response?.Result is null)
            {

                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            Questionnaire = response.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion

    private void NavigateToClassDetails(Guid classId)
    {
        NavigationManager.NavigateTo($"/client/assigned-trainings/client-class-details/{classId}");
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