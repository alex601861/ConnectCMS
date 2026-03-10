using MudBlazor;
using CMSTrain.Client.Models.Base;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Requests.TrainingCandidate;
using CMSTrain.Client.Models.Responses.TrainingCandidate;

namespace CMSTrain.Client.Pages.Candidate.Training;

public partial class AvailableTrainingsList : ComponentBase
{
    [Parameter] public int RequestAction { get; set; }
    
    [Parameter] public bool IsRemarksRequired { get; set; }
    
    [Parameter] public EventCallback OnAvailableTrainingsCountUpdate { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        await GetAllTrainingsForCandidate();
    }
    
    // TODO: Implementation of Component Based Through Out
    private bool IsDisplayedAsGrid { get; set; } = true;

    private void HandleDisplayFormatChange(bool value)
    {
        IsDisplayedAsGrid = value;
        
        StateHasChanged();
    }

    #region Search
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
        
        await GetAllTrainingsForCandidate();
    }
    #endregion

    #region Training Details
    private CollectionDto<GetAllTrainingsForCandidate>? TrainingDetails { get; set; }
    
    private int PageNumber { get; set; } = Constants.Pagination.Page;
    
    private int PageSize { get; set; } = Constants.Pagination.Size; 

    private async Task UpdatePageNumber(int pageNumber)
    {
        PageNumber = pageNumber;
        
        TrainingDetails = null; 
        
        await GetAllTrainingsForCandidate();
    }
    
    private async Task UpdatePageSize(int pageSize)
    {
        PageNumber = 1;
        
        PageSize = pageSize;
        
        TrainingDetails = null; 
        
        await GetAllTrainingsForCandidate();
    }
    
    private async Task GetAllTrainingsForCandidate()
    {
        try
        {
            var response = await TrainingService.GetAllTrainingsForCandidate(RequestAction, PageNumber, PageSize, Search);

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            TrainingDetails = response;
            
            foreach (var training in TrainingDetails.Result)
            {
                training.ImageUrl = training.ImageUrl != null 
                    ? FileManager.FetchFileUrl(training.ImageUrl, Constants.FilePath.TrainingsImagesFilePath) 
                    : "images/dummy-img.png";
            }
            
            StateHasChanged();
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion
    
    #region Training Request
    private bool SelfRequest { get; set; }

    private SelfCandidateAssignmentDto CandidateAssignment { get; set; } = new();

    private void OpenCloseSelfRequestModal()
    {
        SelfRequest = !SelfRequest;

        StateHasChanged();
    }
    
    private void OpenSelfRequestModal(Guid trainingId)
    {
        CandidateAssignment = new SelfCandidateAssignmentDto()
        {
            TrainingId = trainingId
        };

        OpenCloseSelfRequestModal();
    }

    private async Task OnSelfTrainingRequest(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseSelfRequestModal();
            return;
        }
        
        try
        {
            var result = await TrainingCandidateService.SelfCandidateAssignment(CandidateAssignment);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Warning, Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    OpenCloseSelfRequestModal();
                    await GetAllTrainingsForCandidate();
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
                    await OnAvailableTrainingsCountUpdate.InvokeAsync();
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

    #region Cancel Request
    private bool IsCancelRequestModalOpen { get; set; } = new();
    
    private SelfCandidateAssignmentDto CancelTrainingRequest { get; set; } = new();

    private void OpenCloseCancelRequestModal()
    {
        IsCancelRequestModalOpen = !IsCancelRequestModalOpen;
        
        StateHasChanged();
    }

    private void OpenCancelRequestModal(Guid trainingCandidateId)
    {
        CancelTrainingRequest = new SelfCandidateAssignmentDto()
        {
            TrainingId = trainingCandidateId
        };

        OpenCloseCancelRequestModal();
    }

    private async Task OnCancelTrainingRequest(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseCancelRequestModal();
            return;
        }

        try
        {
            var result = await TrainingCandidateService.CancelTrainingRequest(CancelTrainingRequest.TrainingId);
            
            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Warning, Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    OpenCloseCancelRequestModal();
                    await GetAllTrainingsForCandidate();
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
                    await OnAvailableTrainingsCountUpdate.InvokeAsync();
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
    
    #region Navigation
    private void NavigateToAssignedTrainingDetails(Guid trainingId)
    {
        NavigationManager.NavigateTo($"candidate/assigned-trainings/training-details/{trainingId}");
    }
    
    private void NavigateToUnassignedTrainingDetails(Guid trainingId)
    {
        NavigationManager.NavigateTo($"candidate/available-trainings/training-details/{trainingId}");
    }
    #endregion
}