using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Responses.Heading;

namespace CMSTrain.Client.Pages.Administrator.Analysis.Assessment;

public partial class Headings
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
        Layout.PageTitle = PageTitle.Heading;
    }
    #endregion

    #region Headings Count
    private GetHeadingCountDto HeadingCount { get; set; } = new();

    private async Task GetHeadersCount()
    {
        var result = await HeadingService.GetAllHeadingCount(FacetType.Heading, InspectionType.PersonalAssessment);

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