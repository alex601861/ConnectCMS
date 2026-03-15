using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.Heading;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CMSTrain.Client.Pages.Administrator.Analysis.Personality;

public partial class Facets
{
    private int ActivePanelIndex { get; set; }

    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        
        await GetHeadersCount();
    }

    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.Facet;
    }
    #endregion
    
    #region Headings Count
    private GetHeadingCountDto HeadingCount { get; set; } = new();

    private async Task GetHeadersCount()
    {
        var result = await HeadingService.GetAllHeadingCount(FacetType.Facet, InspectionType.PersonalityTest);

        if (result?.Result is null)
        {
            SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        HeadingCount = result.Result;
    }
    #endregion

    #region Component Module Update on Count 
    private async Task HandleHeadersCount()
    {
        await GetHeadersCount();

        StateHasChanged();
    }
    #endregion
}