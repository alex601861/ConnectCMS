using CMSTrain.Client.Layout.Application;
using MudBlazor;
using CMSTrain.Client.Models.Base;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.Strategy;

namespace CMSTrain.Client.Pages.Candidate.Strategy;

public partial class StrategicTraitDetails : ComponentBase
{
    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        
        await GetStrategicTraitQuestionnaireResponses();
    }
    
    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.StrategicTraitDetails;
    }
    #endregion

    #region Filter
    private MudDateRangePicker Picker { get; set; } = new();
    
    private DateRange? DateRange { get; set; } = new(DateTime.Now.Date, DateTime.Now.AddDays(5).Date);

    private DateTime? StartDate { get; set; }

    private DateTime? EndDate { get; set; }
    
    private async Task OnDateRangeFilter(bool isClosed)
    {
        StartDate = isClosed ? DateRange == null ? null : DateRange.Start : null;
        
        EndDate = isClosed ? DateRange == null ? null : DateRange.End : null;
        
        await GetStrategicTraitQuestionnaireResponses();

        if (isClosed)
        {
            await Picker.CloseAsync();
        }
        else
        {
            await Picker.ClearAsync();
        }
        
        StateHasChanged();
    }
    #endregion
    
    private CollectionDto<GetStrategyTraitQuestionnaireDto>? StrategicTraitQuestionnaireResponses { get; set; }
    
    private int PageNumber { get; set; } = Constants.Pagination.Page;
    
    private int PageSize { get; set; } = Constants.Pagination.Size; 
    
    private async Task UpdatePageNumber(int pageNumber)
    {
        PageNumber = pageNumber;
        
        StrategicTraitQuestionnaireResponses = null;
        
        await GetStrategicTraitQuestionnaireResponses();
    }

    private async Task UpdatePageSize(int pageSize)
    {
        PageNumber = 1;
        
        PageSize = pageSize;
        
        StrategicTraitQuestionnaireResponses = null;
        
        await GetStrategicTraitQuestionnaireResponses();
    }

    private async Task GetStrategicTraitQuestionnaireResponses()
    {
        try
        {
            var result = await StrategicTraitService.GetStrategyTraitQuestionnaireResponses(PageNumber, PageSize, StartDate, EndDate);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            StrategicTraitQuestionnaireResponses = result;
            
            StateHasChanged();
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    
    private void NavigateToQuestionnaire()
    {
        NavigationManager.NavigateTo("/strategic-trait-questionnaire");
    }
    
    private void NavigateToResponseDetails(Guid responseId)
    {
        NavigationManager.NavigateTo($"/strategic-trait-responses/{responseId}");
    }
}