using MudBlazor;
using CMSTrain.Client.Models.Base;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Extensions;
using CMSTrain.Client.Models.Responses.Training;
using CMSTrain.Client.Models.Requests.TrainingCandidate;
using CMSTrain.Client.Models.Responses.TrainingCandidate;

namespace CMSTrain.Client.Pages.Administrator.Request;

public partial class RequestDetails
{
    [Parameter] public int RequestAction { get; set; }

    [Parameter] public Guid TrainingId { get; set; }

    [Parameter] public EventCallback<Guid> OnTrainingSelected { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        await GetAllTrainings();
        await HandleTrainingCandidatesRequests();
    }
    
    #region Search and Filter
    private string _search = string.Empty;
    
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
        
        await HandleTrainingCandidatesRequests();
        
        StateHasChanged();
    }

    private async Task OnTrainingFilter()
    {
        PageSize = Constants.Pagination.Size;

        PageNumber = Constants.Pagination.Page;
        
        CandidateTrainingDetails = null;
        
        await OnTrainingSelected.InvokeAsync(TrainingId);
        
        await HandleTrainingCandidatesRequests();
        
        StateHasChanged();
    }
    #endregion
    
    #region Training Details
    private List<GetTrainingDto> Trainings { get; set; } = [];

    private async Task GetAllTrainings()
    {
        try
        {
            var response = await TrainingService.GetAllTrainings(Constants.StatusAction.All);

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            
                return;
            }

            Trainings = response.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    
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
    
    private void OpenTrainingDetailsModal(Guid trainingCandidateId)
    {
        CandidateRequestDetails = CandidateTrainingDetails?.Result.FirstOrDefault(x => x.TrainingCandidateId == trainingCandidateId) ?? new GetAllTrainingRequestsForAdmin();

        CandidateRequestDetails.TrainingDetails.ImageUrl = 
            !string.IsNullOrEmpty(CandidateRequestDetails.TrainingDetails.ImageUrl) 
                ? FileManager.FetchFileUrl(CandidateRequestDetails.TrainingDetails.ImageUrl, Constants.FilePath.TrainingsImagesFilePath)
                : "images/dummy-img.png";
        
        OpenCloseTrainingDetailsModal();
    }
    #endregion
    
    #region Training Requests
    private CollectionDto<GetAllTrainingRequestsForAdmin>? CandidateTrainingDetails { get; set; }
    
    private int PageNumber { get; set; } = Constants.Pagination.Page;
    
    private int PageSize { get; set; } = Constants.Pagination.Size; 

    private async Task UpdatePageNumber(int pageNumber)
    {
        PageNumber = pageNumber;
        
        CandidateTrainingDetails = null;
        
        await HandleTrainingCandidatesRequests();
    }
    
    private async Task UpdatePageSize(int pageSize)
    {
        PageNumber = 1;
        
        PageSize = pageSize;
        
        CandidateTrainingDetails = null;
        
        await HandleTrainingCandidatesRequests();
    }
    
    private async Task HandleTrainingCandidatesRequests()
    {
        try
        {
            var result = await TrainingCandidateService.GetAllTrainingRequestsForAdmin(RequestAction, PageNumber, PageSize, Search, ExtensionMethods.ToNullOrValue(TrainingId));

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    CandidateTrainingDetails = result;
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
    }
    #endregion
    
    #region Approval Matrix 
    private bool IsApproveRejectModalOpen { get; set; }

    private bool _isRequestHandlingDisabled;

    private bool IsRequestHandlingDisabled
    {
        get => _isRequestHandlingDisabled;
        set => _isRequestHandlingDisabled = value;
    }
    
    private void HandleRequestAssignmentBusySubmitting(bool isBusySubmitting)
    {
        IsRequestHandlingDisabled = isBusySubmitting;
        
        StateHasChanged();
    }
    
    private ApproveRejectRequestDto ApproveReject { get; set; } = new();

    private void OpenApproveRejectModal(Guid assigmentId, bool isApproved)
    {
        OpenCloseApproveRejectModal();
        
        ApproveReject = new()
        {
            TrainingCandidateId = assigmentId,
            IsApproved = isApproved
        };
    }
    
    private async Task HandleApproveReject(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseApproveRejectModal();
            return;
        }

        try
        {
            HandleRequestAssignmentBusySubmitting(true);

            var result = await TrainingCandidateService.ApprovalRejectTrainingCandidateRequest(ApproveReject);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await HandleTrainingCandidatesRequests();
                    await OnTrainingSelected.InvokeAsync(TrainingId);
                    OpenCloseApproveRejectModal();
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
            HandleRequestAssignmentBusySubmitting(false);
        }
    }

    private void OpenCloseApproveRejectModal()
    {
        IsApproveRejectModalOpen = !IsApproveRejectModalOpen;
        
        StateHasChanged();
    }
    #endregion
}