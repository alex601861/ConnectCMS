using CMSTrain.Application.DTOs.Count;
using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Constants;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CMSTrain.Client.Pages.Candidate.Training;

public partial class AssignedTrainings : ComponentBase
{
    private int ActivePanelIndex { get; set; }

    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        await GetAllAssignedTrainingsCount();
    }

    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.AssignedTrainings;
    }
    #endregion

    #region AssignedTrainingCount Details
    private AssignedTrainingCountDto AssignedTrainingCountDto { get; set; } = new();

    private async Task GetAllAssignedTrainingsCount()
    {
        try
        {
            var response = await TrainingService.GetAssignedTrainingCountsForCandidate();

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            AssignedTrainingCountDto = response.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion 
}