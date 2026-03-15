using Blazorise.Extensions;
using MudBlazor;
using CMSTrain.Client.Models.Base;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Requests.TrainingCandidate;
using CMSTrain.Client.Models.Responses.Candidate;
using CMSTrain.Client.Models.Responses.Identity;

namespace CMSTrain.Client.Pages.Client.Training.Details;

public partial class CandidateDetails : ComponentBase
{
    [Parameter] public Guid TrainingId { get; set; }
    
    [Parameter] public EventCallback OnCandidateDetailsCountUpdate { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await GetClientUserDetails();
        
        await GetApprovedCandidateDetails();
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
    private int CandidateType { get; set; } = Constants.CandidateType.Organizational;
    
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

    private async Task OnCandidateFilter()
    {
        PageSize = Constants.Pagination.Size;

        PageNumber = Constants.Pagination.Page;

        CandidateDetail = null;
        
        await GetApprovedCandidateDetails();
    }
    #endregion
    
    #region Candidate Assignments
    private bool IsAdminRequestAssignmentModalOpen { get; set; }

    private IReadOnlyCollection<Guid> SelectedCandidateIds { get; set; } = [];

    private List<GetCandidateDetailsDto> UnassignedCandidates { get; set; } = [];

    private bool IsCandidateNotAssignedDisabled =>
        SelectedCandidateIds.IsNullOrEmpty();

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
    
    private async Task GetUnassignedCandidatesForTraining()
    {
        try
        {
            var response = await TrainingCandidateService.GetAllUnassignedClientCandidatesForTraining(TrainingId);

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
    
    private async Task CloseClientRequestModal(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseRequestModal();
            
            return;
        }
        
        try
        {
            AssignCandidatesModel = new()
            {
                TrainingId = TrainingId,
                CandidateIds = SelectedCandidateIds.ToList()
            };
                
            var result = await TrainingCandidateService.AdminCandidateAssignment(AssignCandidatesModel);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    OpenCloseRequestModal();
                    await GetApprovedCandidateDetails();
                    await OnCandidateDetailsCountUpdate.InvokeAsync();
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
    }
    
    private static string GetUserDisplayName(GetCandidateDetailsDto user)
    {
        return $"{user.Name} ({user.EmailAddress})";
    }
    #endregion
    

    #region Candidate Details
    private UserDetail ClientUserDetails { get; set; } = new();

    private async Task GetClientUserDetails()
    {
        try
        {
            var result = await ProfileService.GetUserProfile();
            
            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }
            
            ClientUserDetails = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
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
            var response = CandidateType == Constants.CandidateType.All 
                ? await TrainingCandidateService.GetAllApprovedCandidatesForTraining(TrainingId, PageNumber, PageSize, Search)
                : await TrainingCandidateService.GetAllOrganizationalCandidatesForTraining(TrainingId, PageNumber, PageSize, Search);

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
    
    #region Navigation to Candidate Details
    private void NavigateToCandidateDetails(Guid trainingCandidateId)
    {
        NavigationManager.NavigateTo($"trainings/candidate/{trainingCandidateId}");
    }
    #endregion
}