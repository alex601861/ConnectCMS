using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.TrainingInspection;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CMSTrain.Client.Pages.Client.Training.Details;

public partial class InspectionDetails
{
    [Parameter] public Guid TrainingId { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        await GetAllAssignedTrainingInspectionDetails();
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
        
        await GetAllAssignedTrainingInspectionDetails();
    }
    #endregion
    
    #region Training Inspection Questionnaires 
    private CollectionDto<GetTrainingInspectionDto>? Inspections { get; set; }
    
    private int PageNumber { get; set; } = Constants.Pagination.Page;
    
    private int PageSize { get; set; } = Constants.Pagination.Size; 

    private async Task UpdatePageNumber(int pageNumber)
    {
        PageNumber = pageNumber;
        
        Inspections = null;
        
        await GetAllAssignedTrainingInspectionDetails();
    }

    private async Task UpdatePageSize(int pageSize)
    {
        PageNumber = 1;
        
        PageSize = pageSize;
        
        Inspections = null;
        
        await GetAllAssignedTrainingInspectionDetails();
    }
    
    private async Task GetAllAssignedTrainingInspectionDetails()
    {
        try
        {
            var result = await TrainingInspectionService.GetAllAssignedTrainingInspectionsForClient(TrainingId, PageNumber, PageSize, Search);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }
            
            Inspections = result;
            
            StateHasChanged();
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion
    
    #region Navigation
    private void NavigateToQuestionDetailsForm(Guid questionnaireId)
    {
        NavigationManager.NavigateTo($"/client/questionnaire-view-form/{questionnaireId}");
    }
    #endregion
}