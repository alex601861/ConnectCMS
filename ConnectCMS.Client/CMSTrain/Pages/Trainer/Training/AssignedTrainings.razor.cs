using CMSTrain.Application.DTOs.Count;
using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Constants;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CMSTrain.Client.Pages.Trainer.Training;

public partial class AssignedTrainings : ComponentBase
{
    private int ActivePanelIndex { get; set; }

    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        await GetAllAssignedTrainingCountForTrainers();
    }

    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.AssignedTrainings;
    }
    #endregion

    #region Assigned Training Counts
    private AssignedTrainingCountDto AssignedTrainingCountDto { get; set; } = new();
    
    private async Task GetAllAssignedTrainingCountForTrainers()
    {
        try
        {
            var response = await TrainingService.GetAllAssignedTrainingCountForTrainers();

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