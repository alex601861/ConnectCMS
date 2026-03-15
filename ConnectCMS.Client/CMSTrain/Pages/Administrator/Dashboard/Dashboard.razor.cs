using ApexCharts;
using MudBlazor;
using CMSTrain.Client.Models.Constants;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Responses.Dashboard;
using CMSTrain.Client.Models.Responses.Identity;

namespace CMSTrain.Client.Pages.Administrator.Dashboard;

public partial class Dashboard : ComponentBase
{
    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        GetTrainingYears();
        
        await GetUserName();
        await GetTopTraining();
        await GetUpcomingTrainings();
        await GetAdminDashboardCount();
        await GetTrainingRequestSummary(SelectedYear);
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

        await GetAdminDashboardCount();
    }
    #endregion
    
    #region Dashboard Count
    private GetAdminCountDto AdminDashboardCount { get; set; } = new();

    private async Task GetAdminDashboardCount()
    {
        var response = await DashboardService.GetAdminDashboardCount(TimePeriod);

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        AdminDashboardCount = response.Result;
    }
    #endregion

    #region Popular Trainings
    private List<GetPopularTrainingDto> PopularTrainings { get; set; } = [];

    private async Task GetTopTraining()
    {
        var response = await DashboardService.GetPopularTrainings();

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        PopularTrainings = response.Result;

        foreach (var training in PopularTrainings)
        {
            if (!string.IsNullOrEmpty(training.ImageUrl))
            {
                training.ImageUrl =
                    FileManager.FetchFileUrl(training.ImageUrl, Constants.FilePath.TrainingsImagesFilePath);
            }
        }
    }
    #endregion

    #region Upcoming Trainings
    private List<GetUpcomingTrainingDto> UpcomingTrainings { get; set; } = [];

    private async Task GetUpcomingTrainings()
    {
        var response = await DashboardService.GetUpcomingTrainings();

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        UpcomingTrainings = response.Result;

        foreach (var upcomingTraining in UpcomingTrainings)
        {
            if (!string.IsNullOrEmpty(upcomingTraining.ImageUrl))
                upcomingTraining.ImageUrl = FileManager.FetchFileUrl(upcomingTraining.ImageUrl,
                    Constants.FilePath.TrainingsImagesFilePath);
        }
    }
    #endregion

    #region Training Details with Start Dates
    private bool IsTriggered { get; set; }

    private List<int> Years { get; set; } = new();

    private int SelectedYear { get; set; } = DateTime.UtcNow.Year;
    
    private ApexChart<TrainingStartDateSummary>? ChartData { get; set; }
    
    private readonly ApexChartOptions<TrainingStartDateSummary> _chartOptions = new()
    {
        Tooltip = new Tooltip
        {
            Shared = false,
            Y = new TooltipY
            {
                Formatter = new string("""
                                       function(value, { series, seriesIndex, dataPointIndex, w }) {
                                           const names = w.config.series[seriesIndex].group.split(',');
                                           const title = names[dataPointIndex]?.trim() || 'No Title';
                                       
                                           const getOrdinalSuffix = (num) => {
                                               const suffixes = ["th", "st", "nd", "rd"];
                                               const v = num % 100;
                                               return num + (suffixes[(v - 20) % 10] || suffixes[v] || suffixes[0]);
                                           };
                                       
                                           const ordinalDate = getOrdinalSuffix(value);
                                           return title + ': ' + ordinalDate;
                                       }
                                       """)
            }
        },
        DataLabels = new DataLabels
        {
            Enabled = true,
            OffsetY = -20,
        },
        Legend = new Legend
        {
            Show = false
        }
    };
    
    private GetTrainingRequestsSummaryDto TrainingRequestsSummary { get; set; } = new();

    private void GetTrainingYears()
    {
        Years = Enumerable.Range(2024, SelectedYear - 2024 + 1).ToList();
    }

    private async Task OnYearChanged(int value)
    {
        SelectedYear = value;
        
        await GetTrainingRequestSummary(SelectedYear);
        
        if (ChartData != null)
        {
            await ChartData.UpdateSeriesAsync();
            
            await InvokeAsync(StateHasChanged);
        }
    }
    
    private async Task GetTrainingRequestSummary(int year)
    {
        var response = await DashboardService.GetTrainingRequestSummary(year);

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        TrainingRequestsSummary = response.Result;
        
        StateHasChanged();
    }
    #endregion

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