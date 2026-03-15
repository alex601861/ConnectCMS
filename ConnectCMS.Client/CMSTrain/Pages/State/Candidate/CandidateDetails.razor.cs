using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Responses.Candidate;
using CMSTrain.Client.Models.Requests.Certification;
using CMSTrain.Client.Models.Responses.TrainingCandidate;

namespace CMSTrain.Client.Pages.State.Candidate;

public partial class CandidateDetails : ComponentBase
{
    [Parameter]
    public Guid TrainingCandidateId { get; set; }

    private int ActivePanelIndex { get; set; } = -1;

    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();

        var trainingCandidateDetails = await GetTrainingCandidateDetails();

        await GetCandidateDetails(trainingCandidateDetails.CandidateId);
        
        ActivePanelIndex = 0;
        
        StateHasChanged();

        await GetNavigationDetails();
    }

    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.CandidateDetails;
    }
    #endregion
    
    #region Training Candidate
    private TrainingCandidateAssignmentDetailsDto TrainingCandidateAssignment { get; set; } = new();
    
    private async Task<TrainingCandidateAssignmentDetailsDto> GetTrainingCandidateDetails()
    {
        try
        {
            var result = await TrainingCandidateService.GetTrainingCandidateAssignmentDetails(TrainingCandidateId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Warning, Variant.Outlined);
                return new TrainingCandidateAssignmentDetailsDto();
            }

            TrainingCandidateAssignment = result.Result;

            return TrainingCandidateAssignment;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }

        return new TrainingCandidateAssignmentDetailsDto();
    }
    #endregion
    
    #region Candidate
    private GetCandidateDetailsDto CandidateDetail { get; set; } = new();

    private async Task GetCandidateDetails(Guid candidateId)
    {
        try
        {
            var result = await CandidateService.GetCandidateDetailsById(candidateId);
            
            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Warning, Variant.Outlined);
                return;
            }

            CandidateDetail = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion

    #region Training Assignment Details
    private bool IsTrainingDetailsOpen { get; set; }

    private void OpenCloseTrainingDetailsModal()
    {
        IsTrainingDetailsOpen = !IsTrainingDetailsOpen;
        
        if (IsTrainingDetailsOpen == false)
        {
            CandidateRequestDetails = new GetAllTrainingRequestsForAdmin();
        }
        
        StateHasChanged();
    }

    private GetAllTrainingRequestsForAdmin CandidateRequestDetails { get; set; } = new();
    
    private async Task OpenTrainingDetailsModal()
    {
        try
        {
            var result =
                await TrainingCandidateService.GetApprovedTrainingCandidateAssignmentDetails(TrainingCandidateId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                
                return;
            }

            CandidateRequestDetails = result.Result;
            
            CandidateRequestDetails.TrainingDetails.ImageUrl = 
                !string.IsNullOrEmpty(CandidateRequestDetails.TrainingDetails.ImageUrl) 
                    ? FileManager.FetchFileUrl(CandidateRequestDetails.TrainingDetails.ImageUrl, Constants.FilePath.TrainingsImagesFilePath)
                    : "images/dummy-img.png";
        
            OpenCloseTrainingDetailsModal();
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion
    
    #region Training Candidate Questionnaires (with Subordinates)
    // private List<GetTrainingCandidateInspectionDto> TrainingCandidateInspections { get; set; } = [];

    // private async Task GetTrainingCandidateInspections(Guid trainingCandidateId)
    // {
    //     try
    //     {
    //         var result = await TrainingInspectionService.GetTrainingCandidateInspections(trainingCandidateId);
    //         
    //         if (result?.Result is null)
    //         {
    //             SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Warning, Variant.Outlined);
    //             return;
    //         }
    //
    //         TrainingCandidateInspections = result.Result;
    //     }
    //     catch (Exception ex)
    //     {
    //         SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
    //     }
    // }
    #endregion

    #region Role Access & Details
    private string TrainingNavigation { get; set; } = string.Empty;

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

            TrainingNavigation = role.Name switch
            {
                Constants.Roles.SuperAdmin =>
                    $"/trainings/admin/training-details/{TrainingCandidateAssignment.TrainingId}/3",
                Constants.Roles.Admin =>
                    $"/trainings/admin/training-details/{TrainingCandidateAssignment.TrainingId}/3",
                Constants.Roles.Client =>
                    $"/client/assigned-trainings/training-details/{TrainingCandidateAssignment.TrainingId}/3",
                Constants.Roles.Trainer =>
                    $"/trainer/assigned-trainings/training-details/{TrainingCandidateAssignment.TrainingId}/3",
                Constants.Roles.Candidate =>
                    $"/candidate/assigned-trainings/training-details/{TrainingCandidateAssignment.TrainingId}/3",
                _ => TrainingNavigation
            };
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion
}