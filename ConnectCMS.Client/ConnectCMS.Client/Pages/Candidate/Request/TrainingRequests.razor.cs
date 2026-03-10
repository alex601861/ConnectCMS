using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.Count;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CMSTrain.Client.Pages.Candidate.Request;

public partial class TrainingRequests : ComponentBase
{
    private int ActivePanelIndex { get; set; } = 0;

    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        
        await GetTrainingRequestCountsForCandidate();
    }

    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.MyRequests;
    }
    #endregion

    private ApprovalMatrixCountDto ApprovalMatrixCountDto { get; set; } = new();

    private async Task GetTrainingRequestCountsForCandidate()
    {
        try
        {
            var result = await TrainingCandidateService.GetTrainingRequestCountsForCandidate();

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            ApprovalMatrixCountDto = result.Result;

        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
}