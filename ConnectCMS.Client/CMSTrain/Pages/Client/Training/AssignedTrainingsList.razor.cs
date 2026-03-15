using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.TrainingCandidate;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CMSTrain.Client.Pages.Client.Training;

public partial class AssignedTrainingsList : ComponentBase
{
    [Parameter] public int StatusAction { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        await GetAllTrainingsForClient();
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
        
        await GetAllTrainingsForClient();
    }
    #endregion
    
    #region Get All Trainings for Client Organizations
    private CollectionDto<GetAllTrainingsForClient>? TrainingDetails { get; set; }
    
    private int PageNumber { get; set; } = Constants.Pagination.Page;
    
    private int PageSize { get; set; } = Constants.Pagination.Size; 

    private async Task UpdatePageNumber(int pageNumber)
    {
        PageNumber = pageNumber;
        
        TrainingDetails = null;
        
        await GetAllTrainingsForClient();
    }
    
    private async Task UpdatePageSize(int pageSize)
    {
        PageNumber = 1;
        
        PageSize = pageSize;
        
        TrainingDetails = null;
        
        await GetAllTrainingsForClient();
    }
    
    private async Task GetAllTrainingsForClient()
    {
        try
        {
            var response = await TrainingService.GetAllAssignedTrainingsForClient(StatusAction, PageNumber, PageSize, Search);

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
    
    private void NavigateToAssignedTrainingDetails(Guid trainingId)
    {
        NavigationManager.NavigateTo($"client/assigned-trainings/training-details/{trainingId}");
    }
}