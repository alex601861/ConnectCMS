using Blazorise.Extensions;
using MudBlazor;
using CMSTrain.Client.Models.Base;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Requests.Email;
using CMSTrain.Client.Models.Responses.Candidate;
using CMSTrain.Client.Models.Requests.TrainingCandidate;

namespace CMSTrain.Client.Pages.Administrator.Training.Details;

public partial class CandidateDetails : ComponentBase
{
    [Parameter] public Guid TrainingId { get; set; }
    
    [Parameter] public EventCallback OnCandidateDetailsCountUpdate { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await GetApprovedCandidateDetails();
    }
    
    // TODO: Implementation of Component Based Through Out
    private bool IsDisplayedAsGrid { get; set; } = true;

    private void HandleDisplayFormatChange(bool value)
    {
        IsDisplayedAsGrid = value;
        
        StateHasChanged();
    }

    #region Search and Filter
    private string _search = string.Empty;
    private int CandidateType { get; set; } = Constants.CandidateType.All;
    
    private string Search
    {
        get => _search;
        set
        {
            if (_search == value) return;
            _search = value;
            _ = OnSearchInputAsync(_search);
        }
    }
    
    private async Task OnSearchInputAsync(string search)
    {
        Search = search;
        
        PageSize = Constants.Pagination.Size;

        PageNumber = Constants.Pagination.Page;
        
        await GetApprovedCandidateDetails();
    }
    #endregion

    #region Candidate Details
    private CollectionDto<GetApprovedCandidateDetailsDto>? CandidateDetail { get; set; }
    
    private int PageNumber { get; set; } = Constants.Pagination.Page;
    
    private int PageSize { get; set; } = Constants.Pagination.Size; 

    private async Task UpdatePageNumber(int pageNumber)
    {
        PageNumber = pageNumber;

        CandidateDetail = null;
        
        await GetApprovedCandidateDetails();
    }

    private async Task UpdatePageSize(int pageSize)
    {
        PageNumber = 1;
        
        PageSize = pageSize;

        CandidateDetail = null;
        
        await GetApprovedCandidateDetails();
    }
    
    private async Task GetApprovedCandidateDetails()
    {
        try
        {
            var response = await TrainingCandidateService.GetAllApprovedCandidatesForTraining(TrainingId, PageNumber, PageSize, Search);

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            CandidateDetail = response;
            
            StateHasChanged();
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion

    #region Candidate Assignments
    private bool IsAdminRequestAssignmentModalOpen { get; set; }

    private IReadOnlyCollection<Guid> SelectedCandidateIds { get; set; } = [];

    private List<GetCandidateDetailsDto> UnassignedCandidates { get; set; } = [];

    private bool _isCandidateNotAssignedDisabled;

    private bool IsCandidateNotAssignedDisabled
    {
        get => _isCandidateNotAssignedDisabled || 
               SelectedCandidateIds.IsNullOrEmpty();
        set => _isCandidateNotAssignedDisabled = value;
    }
    
    private async Task OpenUnassignedCandidateOpenModal()
    { 
        await GetUnassignedCandidatesForTraining();
        
        OpenCloseRequestModal();
    }
    
    private void OpenCloseRequestModal()
    {
        IsAdminRequestAssignmentModalOpen = !IsAdminRequestAssignmentModalOpen;
        
        StateHasChanged();
    }

    private void HandleAssignmentBusySubmitting(bool isBusySubmitting)
    {
        IsCandidateNotAssignedDisabled = isBusySubmitting;
        
        StateHasChanged();
    }
    
    private async Task GetUnassignedCandidatesForTraining()
    {
        try
        {
            var response = await TrainingCandidateService.GetAllUnassignedCandidatesForTraining(TrainingId);

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            UnassignedCandidates = response.Result;

            StateHasChanged();
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    
    private AssignCandidatesDto AssignCandidatesModel { get; set; } = new();
    
    private async Task AssignCandidateRequests(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseRequestModal();
            
            return;
        }

        try
        {
            HandleAssignmentBusySubmitting(true);

            AssignCandidatesModel = new()
            {
                TrainingId = TrainingId,
                CandidateIds = SelectedCandidateIds.ToList()
            };

            var result = await TrainingCandidateService.AdminCandidateAssignment(AssignCandidatesModel);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await GetApprovedCandidateDetails();
                    await OnCandidateDetailsCountUpdate.InvokeAsync();
                    await CandidateAssignmentEmailConfirmation(AssignCandidatesModel.CandidateIds);
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
                    break;
                case StatusCode.Status404NotFound:
                case StatusCode.Status400BadRequest:
                case StatusCode.Status401Unauthorized:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Warning, Variant.Outlined);
                    break;
                case StatusCode.Status500InternalServerError:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Error, Variant.Outlined);
                    break;
            }
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
        finally
        {
            HandleAssignmentBusySubmitting(false);
        }
    }
    
    private async Task CandidateAssignmentEmailConfirmation(List<Guid> candidateIds)
    {
        try
        {
            HandleAssignmentBusySubmitting(true);

            var trainingAssignmentRequest = new TrainingRequestsActionRequestDto()
            {
                TrainingId = TrainingId,
                RequestActions = candidateIds.Select(x => new RequestAction()
                {
                    UserId = x,
                    Remarks = "",
                    IsApproved = true
                }).ToList()
            };

            var result = await EmailConfirmationService.TrainingRequestAction(trainingAssignmentRequest);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
                    break;
                case StatusCode.Status400BadRequest:
                case StatusCode.Status401Unauthorized:
                case StatusCode.Status404NotFound:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Warning, Variant.Outlined);
                    break;
                case StatusCode.Status500InternalServerError:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Error, Variant.Outlined);
                    break;
            }
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
        finally
        {
            OpenCloseRequestModal();
            HandleAssignmentBusySubmitting(false);
        }
    }
    
    private static string GetUserDisplayName(GetCandidateDetailsDto user)
    {
        return $"{user.Name} ({user.EmailAddress})";
    }
    #endregion

    #region Remove Candidate
    private bool IsRemoveCandidateModalOpen { get; set; }

    private Guid RemoveCandidateId { get; set; }
    
    private void OpenCloseRemoveCandidateModal(Guid trainingCandidateId)
    {
        IsRemoveCandidateModalOpen = !IsRemoveCandidateModalOpen;

        RemoveCandidateId = IsRemoveCandidateModalOpen ? trainingCandidateId : new Guid();
        
        StateHasChanged();
    }
    
    private async Task RemoveCandidateFromTraining(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseRemoveCandidateModal(Guid.Empty);

            return;
        }
        
        try
        {
            var response = await TrainingCandidateService.RemoveCandidateFromTraining(RemoveCandidateId);

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            switch (response.StatusCode)
            {
                case StatusCode.Status200Ok:
                    SnackbarService.ShowSnackbar(response.Message, Severity.Success, Variant.Outlined);
                    await OnCandidateDetailsCountUpdate.InvokeAsync();
                    OpenCloseRemoveCandidateModal(Guid.Empty);
                    await GetApprovedCandidateDetails();
                    break;
                case StatusCode.Status401Unauthorized:
                case StatusCode.Status400BadRequest:
                case StatusCode.Status404NotFound:
                    SnackbarService.ShowSnackbar(response.Message, Severity.Warning, Variant.Outlined);
                    break;
                case StatusCode.Status500InternalServerError:   
                    SnackbarService.ShowSnackbar(response.Message, Severity.Error, Variant.Outlined);
                    break;
            }
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion

    #region Navigation to Candidate Details
    private void NavigateToCandidateDetails(Guid trainingCandidateId)
    {
        NavigationManager.NavigateTo($"trainings/candidate/{trainingCandidateId}");
    }
    #endregion
}