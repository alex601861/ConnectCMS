using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Responses.Count;

namespace CMSTrain.Client.Pages.Administrator.Training;

public partial class View : ComponentBase
{
    [Parameter] public Guid TrainingId { get; set; }

    [Parameter] public int ActivePanelIndex { get; set; }

    private TrainingDetailsCountDto TrainingDetailsCountDto { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        await GetTrainingDetailsCountDto();
    }

    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.TrainingDetails;
    }
    #endregion

    #region Module Counts
    private async Task GetTrainingDetailsCountDto()
    {
        var result = await TrainingService.GetTrainingDetailsCount(TrainingId);

        if (result?.Result is null)
        {
            SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        TrainingDetailsCountDto = result.Result;
    }
    #endregion
    
    #region Component Module Update on Count 
    private async Task HandleTrainingDetailsCounts()
    {
        await GetTrainingDetailsCountDto();
        
        StateHasChanged();
    }
    #endregion
}