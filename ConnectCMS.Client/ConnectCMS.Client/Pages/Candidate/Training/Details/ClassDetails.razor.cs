using MudBlazor;
using CMSTrain.Client.Models.Base;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.Class;

namespace CMSTrain.Client.Pages.Candidate.Training.Details;

public partial class ClassDetails : ComponentBase
{
    [Parameter] public Guid TrainingId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await GetAllClassesForCandidates();
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

        await GetAllClassesForCandidates();
    }

    private int? Status { get; set; }

    private async Task OnClassDetailsFilter()
    {
        PageSize = Constants.Pagination.Size;

        PageNumber = Constants.Pagination.Page;
        
        Classes = null;
        
        await GetAllClassesForCandidates();
    }
    #endregion

    #region Class Details
    private CollectionDto<GetClassForCandidatesDto>? Classes { get; set; }
    
    private int PageNumber { get; set; } = Constants.Pagination.Page;
    
    private int PageSize { get; set; } = Constants.Pagination.Size; 

    private async Task UpdatePageNumber(int pageNumber)
    {
        PageNumber = pageNumber;
        
        Classes = null;
        
        await GetAllClassesForCandidates();
    }

    private async Task UpdatePageSize(int pageSize)
    {
        PageNumber = 1;
        
        PageSize = pageSize;
        
        Classes = null;
        
        await GetAllClassesForCandidates();
    }

    private async Task GetAllClassesForCandidates()
    {
        try
        {
            var response = await ClassService.GetAllClassesForCandidates(TrainingId, PageNumber, PageSize, Search, Status);

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            Classes = response;
            
            foreach (var classes in Classes.Result)
            {
                classes.ImageUrl = classes.ImageUrl != null 
                    ? FileManager.FetchFileUrl(classes.ImageUrl, Constants.FilePath.ClassesImagesFilePath) 
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

    #region Class Details Navigation
    private void NavigateToClassDetails(Guid classId)
    {
        NavigationManager.NavigateTo($"candidate/assigned-trainings/candidate-class-details/{classId}");
    }
    #endregion
}