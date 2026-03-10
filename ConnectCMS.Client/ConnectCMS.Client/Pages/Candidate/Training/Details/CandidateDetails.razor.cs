using MudBlazor;
using CMSTrain.Client.Models.Base;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.Candidate;
using CMSTrain.Client.Models.Responses.TrainingCandidate;

namespace CMSTrain.Client.Pages.Candidate.Training.Details;

public partial class CandidateDetails : ComponentBase
{
    [Parameter] public Guid TrainingId { get; set; }

    [Parameter] public TrainingCandidateAssignmentDetailsDto TrainingCandidateAssignment { get; set; } = new();
    
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

    #region Search
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

    private async Task OnCandidateFilter()
    {
        PageSize = Constants.Pagination.Size;

        PageNumber = Constants.Pagination.Page;
        
        CandidateDetail = null;
        
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
            var response = CandidateType == Constants.CandidateType.All 
                ? await TrainingCandidateService.GetAllColleagueCandidatesForTraining(TrainingId, PageNumber, PageSize, Search)
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
}