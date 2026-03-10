using CMSTrain.Application.DTOs.Count;
using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Constants;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CMSTrain.Client.Pages.Candidate.Training;

public partial class AvailableTrainings : ComponentBase
{
    private int ActivePanelIndex { get; set; }

    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        await GetAvailableTrainingCountsForCandidate();
    }

    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.AvailableTrainings;
    }
    #endregion

    #region Training Count
    private AvailableTrainingCountDto AvailableTrainingCountDto { get; set; } = new();
    
    private async Task GetAvailableTrainingCountsForCandidate()
    {
        try
        {
            var result = await TrainingService.GetAvailableTrainingCountsForCandidate();

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            AvailableTrainingCountDto = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion
    
    #region Component Available Trainings Update on Count 
    private async Task HandleAvailableTrainingCountsForCandidate()
    {
        await GetAvailableTrainingCountsForCandidate();
        
        StateHasChanged();
    }
    #endregion
}