using MudBlazor;
using CMSTrain.Client.Models.Base;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.ClassTrainers;
using CMSTrain.Client.Layout.Application;

namespace CMSTrain.Client.Pages.Trainer.Training;

public partial class AssignedTrainingsList : ComponentBase
{
    [Parameter] public int StatusAction { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        await GetAllAssignedTrainingsForTrainers();
    }
    
    // TODO: Implementation of Component Based Through Out
    private bool IsDisplayedAsGrid { get; set; } = true;

    private void HandleDisplayFormatChange(bool value)
    {
        IsDisplayedAsGrid = value;
        
        StateHasChanged();
    }

    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.AssignedTrainingDetails;
    }
    #endregion

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
        
        await GetAllAssignedTrainingsForTrainers();
    }
    #endregion
    
    #region Training Trainers

    private CollectionDto<GetAssignedTrainingsDto>? Trainings { get; set; }
    
    private int PageNumber { get; set; } = Constants.Pagination.Page;
    
    private int PageSize { get; set; } = Constants.Pagination.Size; 

    private async Task UpdatePageNumber(int pageNumber)
    {
        PageNumber = pageNumber;
        
        Trainings = null;
        
        await GetAllAssignedTrainingsForTrainers();
    }

    private async Task UpdatePageSize(int pageSize)
    {
        PageNumber = 1;
        
        PageSize = pageSize;
        
        Trainings = null;
        
        await GetAllAssignedTrainingsForTrainers();
    }

    private async Task GetAllAssignedTrainingsForTrainers()
    {
        try
        {
            var response = await TrainingService.GetAllAssignedTrainingsForTrainers(StatusAction, PageNumber, PageSize, Search);

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            Trainings = response;

            foreach (var training in Trainings.Result)
            {
                training.ImageUrl = training.ImageUrl != null
                    ? FileManager.FetchFileUrl(training.ImageUrl, Constants.FilePath.TrainingsImagesFilePath)
                    : "images/training-img.jpeg";
            }
        
            StateHasChanged();
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
        
    }

    private void NavigateToAssignedTrainingDetails(Guid trainingId)
    {
        NavigationManager.NavigateTo($"trainer/assigned-trainings/training-details/{trainingId}");
    }

    #endregion
}