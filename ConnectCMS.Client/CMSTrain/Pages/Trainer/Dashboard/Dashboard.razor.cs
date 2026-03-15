using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Service.Extensions;
using CMSTrain.Client.Models.Responses.Identity;
using CMSTrain.Client.Models.Responses.Dashboard;

namespace CMSTrain.Client.Pages.Trainer.Dashboard;

public partial class Dashboard : ComponentBase
{
    private bool IsTriggered { get; set; }

    public string Search { get; set; } = string.Empty;
    
    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        await GetUserName();
        await OnDateChanged();
        await GetAllClasses();
        await GetTrainerTotalClasses();
        await GetTrainerActiveTraining();
        await GetTrainerDashboardCounts();
        await GetTrainerUpcomingClasses();
        await GetTrainerCompletedClasses();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            IsTriggered = !IsTriggered;
        }
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

        await GetTrainerDashboardCounts();
    }
    #endregion

    private GetTrainerDashboardCount TrainerDashboardCount { get; set; } = new();

    private List<GetTotalClasses> TrainerTotalClasses { get; set; } = [];

    private List<GetActiveTrainings> TrainerActiveTraining { get; set; } = [];

    private List<GetClassDetails> TrainerUpcomingClassDetails { get; set; } = [];

    private List<GetClassesForDate> TrainerClassesForDates { get; set; } = [];

    private List<GetClassDetails> TrainerCompletedClasses { get; set; } = [];

    private List<GetAllClassesDto> AllClasses { get; set; } = [];

    private DateTime? DateRequest { get; set; } = DateTime.Now;

    private string FormattedDate { get; set; } = ExtensionMethods.GetDayWithSuffix(DateTime.Now.Day);

    private string FormattedMonth { get; set; } = DateTime.Now.ToString("MMMM");

    private async Task GetTrainerDashboardCounts()
    {
        var response = await DashboardService.GetTrainerDashboardCount(TimePeriod);

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        TrainerDashboardCount = response.Result;
    }

    private async Task GetTrainerTotalClasses()
    {
        var response = await DashboardService.GetTotalClassesForTrainer();

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        TrainerTotalClasses = response.Result;
    }

    private async Task GetTrainerActiveTraining()
    {
        var response = await DashboardService.GetAllActiveTrainings();

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        TrainerActiveTraining = response.Result;

        foreach (var training in TrainerActiveTraining)
        {
            if (!string.IsNullOrEmpty(training.ImageUrl))
                training.ImageUrl =
                    FileManager.FetchFileUrl(training.ImageUrl, Constants.FilePath.TrainingsImagesFilePath);
        }
    }

    private async Task GetTrainerUpcomingClasses()
    {
        var response = await DashboardService.GetUpcomingClasses();

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        TrainerUpcomingClassDetails = response.Result;
    }

    private async Task OnDateChanged()
    {
        var dateOnly = DateOnly.FromDateTime(DateRequest!.Value);

        await GetTrainerClassesForDates(dateOnly);
    }
    
    private async Task GetTrainerClassesForDates(DateOnly requestDate)
    {
        var response = await DashboardService.GetClassesForDates(requestDate);

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        FormattedMonth = requestDate.ToString("MMMM");
        FormattedDate = ExtensionMethods.GetDayWithSuffix(requestDate.Day);

        TrainerClassesForDates = response.Result;
        
        foreach (var classes in TrainerClassesForDates)
        {
            classes.ClassImage = !string.IsNullOrEmpty(classes.ClassImage)
                ? FileManager.FetchFileUrl(classes.ClassImage, Constants.FilePath.ClassesImagesFilePath) 
                : "images/dummy-img.png";
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
        foreach (var status in from classItem in AllClasses 
                 let classDate = classItem.ClassDates.ToDateTimeFromDateOnlyString() 
                 let status = classItem.Status where classDate.Date == date.Date select status)
        {
            switch (status)
            {
                case Constants.Schedule.ScheduledAction:
                    return "mud-theme-info";
                case Constants.Schedule.CompletedAction:
                    return "mud-theme-success";
            }
        }

        return string.Empty;
    }
    
    private async Task GetTrainerCompletedClasses()
    {
        var response = await DashboardService.GetCompletedClasses();

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        TrainerCompletedClasses = response.Result;
    }

    private void NavigateToClassDetails(Guid classId)
    {
        NavigationManager.NavigateTo($"/trainer/assigned-trainings/trainer-class-details/{classId}");
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