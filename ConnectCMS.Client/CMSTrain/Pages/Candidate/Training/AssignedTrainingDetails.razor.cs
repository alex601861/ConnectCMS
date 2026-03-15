using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Responses.Count;
using CMSTrain.Client.Models.Responses.TrainingCandidate;

namespace CMSTrain.Client.Pages.Candidate.Training;

public partial class AssignedTrainingDetails : ComponentBase
{
    [Parameter] public Guid TrainingId { get; set; }
    
    [Parameter] public int ActivePanelIndex { get; set; }

    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        await GetAllTrainingDetailsCount();
        await GetTrainingAssignmentDetails();
    }

    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.AssignedTrainingDetails;
    }
    #endregion

    private TrainingCandidateAssignmentDetailsDto TrainingCandidateAssignment { get; set; } = new();
    
    private async Task GetTrainingAssignmentDetails()
    {
        try
        {
            var result = await TrainingCandidateService.GetTrainingCandidateAssignmentDetailsForTraining(TrainingId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                
                return;
            }

            TrainingCandidateAssignment = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }

    #region Assign Training Details Count 
    private TrainingDetailsCountDto TrainingDetailsCountDto { get; set; } = new();

    private async Task GetAllTrainingDetailsCount()
    {
        try
        {
            var result = await TrainingService.GetTrainingDetailsCountForCandidate(TrainingId);

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