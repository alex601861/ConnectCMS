using CMSTrain.Client.Layout.Application;
using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.Count;

namespace CMSTrain.Client.Pages.Administrator.Training;

public partial class Training : ComponentBase
{
    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        await GetTrainingModuleCount();
    }

    private int ActivePanelIndex { get; set; }

    private TrainingModuleCountDto TrainingModuleCountDto { get; set; } = new();
    
    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.TrainingDetails;
    }
    #endregion

    private async Task GetTrainingModuleCount(bool? isActive = null)
    {
        try
        {
            var result = await TrainingService.GetAllTrainingModuleCount(isActive);
            
            if (result?.Result == null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Warning, Variant.Outlined);
                return;
            }
            
            TrainingModuleCountDto = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    
    #region Component Available Trainings Update on Count 
    private async Task HandleTrainingCounts(bool? isActive)
    {
        await GetTrainingModuleCount(isActive);
        
        StateHasChanged();
    }
    #endregion
}