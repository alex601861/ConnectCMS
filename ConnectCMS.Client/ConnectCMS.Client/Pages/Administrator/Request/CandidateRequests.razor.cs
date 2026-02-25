using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Service.Extensions;
using CMSTrain.Client.Models.Responses.Count;
using CMSTrain.Client.Models.Responses.TrainingCandidate;

namespace CMSTrain.Client.Pages.Administrator.Request;

public partial class CandidateRequests
{
    private int ActivePanelIndex { get; set; }

    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        await GetApprovalMatrixCount();
        await GetTrainingRequestsCountSummary();
    }

    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.CandidateRequest;
    }
    #endregion

    #region Request Summary
    public Guid TrainingId { get; set; }
    
    private GetTrainingRequestsCount GetTrainingRequestsCount { get; set; } = new();

    private async Task GetTrainingRequestsCountSummary()
    {
        try
        {
            var result = await TrainingCandidateService.GetTrainingRequestsCount(ExtensionMethods.ToNullOrValue(TrainingId));

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Warning, Variant.Outlined);
                return;
            }

            GetTrainingRequestsCount = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion

    #region Approval Matrix Count
    private ApprovalMatrixCountDto ApprovalMatrixCountDto { get; set; } = new();
    
    private async Task GetApprovalMatrixCount()
    {
        try
        {
            var result = await TrainingCandidateService.GetApprovalMatrixCount(ExtensionMethods.ToNullOrValue(TrainingId));

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
    #endregion
    
    private async Task HandleTrainingSelection(Guid trainingId)
    {
        TrainingId = trainingId;
        
        await GetApprovalMatrixCount();
        
        await GetTrainingRequestsCountSummary();
        
        StateHasChanged();
    }
}