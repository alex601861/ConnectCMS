using MudBlazor;
using CMSTrain.Client.Models.Base;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.Class;
using CMSTrain.Client.Models.Responses.TrainingCandidate;

namespace CMSTrain.Client.Pages.State.Training.Details;

public partial class ClassDetails : ComponentBase
{
    [Parameter] public Guid TrainingId { get; set; }

    [Parameter] public TrainingCandidateAssignmentDetailsDto TrainingCandidateAssignment { get; set; } = new();
    
    protected override async Task OnInitializedAsync()
    {
        await GetAllClasses();
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

        await GetAllClasses();
    }

    private int? Status { get; set; }

    private async Task OnClassDetailsFilter()
    {
        PageSize = Constants.Pagination.Size;

        PageNumber = Constants.Pagination.Page;
        
        Classes = null;
        
        await GetAllClasses();
    }
    #endregion

    #region Class Details
    private CollectionDto<GetClassDto>? Classes { get; set; }
    
    private int PageNumber { get; set; } = Constants.Pagination.Page;
    
    private int PageSize { get; set; } = Constants.Pagination.Size; 

    private async Task UpdatePageNumber(int pageNumber)
    {
        PageNumber = pageNumber;
        
        Classes = null;
        
        await GetAllClasses();
    }
    
    private async Task UpdatePageSize(int pageSize)
    {
        PageNumber = 1;
        
        PageSize = pageSize;
        
        Classes = null;
        
        await GetAllClasses();
    }
    
    private async Task GetAllClasses()
    {
        try
        {
            var response = await ClassService.GetAllClasses(TrainingId, PageNumber, PageSize, Search, Status);

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            Classes = response;

            foreach (var @class in Classes.Result)
            {
                if (!string.IsNullOrEmpty(@class.ImageUrl))
                    @class.ImageUrl =
                        FileManager.FetchFileUrl(@class.ImageUrl, Constants.FilePath.ClassesImagesFilePath);
            }
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion
}