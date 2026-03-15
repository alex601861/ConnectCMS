using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.TrainingCandidate;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CMSTrain.Client.Pages.Candidate.Request;

public partial class RequestDetails : ComponentBase
{
    [Parameter] public int RequestAction { get; set; } = Constants.RequestAction.Pending;
    
    [Parameter] public bool ShowRemarks { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await HandleCandidateTrainings();
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
        
        await HandleCandidateTrainings();
        
        StateHasChanged();
    }
    #endregion
    
    #region Training Requests
    private CollectionDto<GetAllTrainingRequestsForCandidate>? TrainingRequestForCandidate { get; set; }
    
    private int PageNumber { get; set; } = Constants.Pagination.Page;
    
    private int PageSize { get; set; } = Constants.Pagination.Size; 

    private async Task UpdatePageNumber(int pageNumber)
    {
        PageNumber = pageNumber;
        
        TrainingRequestForCandidate = null;
        
        await HandleCandidateTrainings();
    }

    private async Task UpdatePageSize(int pageSize)
    {
        PageNumber = 1;
        
        PageSize = pageSize;
        
        TrainingRequestForCandidate = null;
        
        await HandleCandidateTrainings();
    }
    
    private async Task HandleCandidateTrainings()
    {
        try
        {
            var result = await TrainingCandidateService.GetAllTrainingRequestsForCandidate(RequestAction, PageNumber, PageSize, Search);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            TrainingRequestForCandidate = result;

            foreach (var trainingRequests in TrainingRequestForCandidate.Result)
            {
                trainingRequests.TrainingDetails.ImageUrl = !string.IsNullOrEmpty(trainingRequests.TrainingDetails.ImageUrl) 
                    ? FileManager.FetchFileUrl(trainingRequests.TrainingDetails.ImageUrl, Constants.FilePath.TrainingsImagesFilePath) 
                    : "images/dummy-img.png";
            }
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion
}