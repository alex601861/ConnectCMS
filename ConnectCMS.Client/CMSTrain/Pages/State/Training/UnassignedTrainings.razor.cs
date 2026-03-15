using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Responses.Count;

namespace CMSTrain.Client.Pages.State.Training;

public partial class UnassignedTrainings : ComponentBase
{
    [Parameter] public Guid TrainingId { get; set; }
    
    private int ActivePanelIndex { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();

        await GetNavigationDetails();
        
        await GetAllTrainingsCount();
    }
    
    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.AvailableTrainings;
    }
    #endregion
    
    private TrainingDetailsCountDto TrainingDetailsCountDto { get; set; } = new();

    private async Task GetAllTrainingsCount()
    {
        try
        {
            var result = await TrainingService.GetTrainingDetailsCount(TrainingId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            TrainingDetailsCountDto = result.Result;

        }
        catch (Exception ex) 
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    
    #region Role Access & Details
    private string UnAssignedTrainingsNavigation { get; set; } = string.Empty;

    private async Task GetNavigationDetails()
    {
        try
        {
            var response = await ProfileService.GetUserRole();

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            var role = response.Result;

            UnAssignedTrainingsNavigation = role.Name switch
            {
                Constants.Roles.Client =>
                    $"/client/available-trainings",
                Constants.Roles.Trainer =>
                    $"/trainer/available-trainings",
                Constants.Roles.Candidate =>
                    $"/candidate/available-trainings",
                _ => UnAssignedTrainingsNavigation
            };
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion
}