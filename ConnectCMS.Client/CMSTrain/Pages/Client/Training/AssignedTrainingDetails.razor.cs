using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Responses.Count;

namespace CMSTrain.Client.Pages.Client.Training;

public partial class AssignedTrainingDetails : ComponentBase
{
    [Parameter] public Guid TrainingId { get; set; }
    
    [Parameter] public int ActivePanelIndex { get; set; }

    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        await GetAllTrainingCountDetails();
    }

    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.AssignedTrainingDetails;
    }
    #endregion

    #region Training Details Count 
    private TrainingDetailsCountDto TrainingDetailsCountDto { get; set; } = new();

    private async Task GetAllTrainingCountDetails()
    {
        try
        {
            var result = await TrainingService.GetTrainingDetailsCountForClient(TrainingId);

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
    #endregion
}