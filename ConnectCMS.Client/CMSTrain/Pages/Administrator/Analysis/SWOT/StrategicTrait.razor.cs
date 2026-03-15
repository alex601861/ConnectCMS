using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Responses.Strategy;

namespace CMSTrain.Client.Pages.Administrator.Analysis.SWOT;

public partial class StrategicTrait
{
    private int ActivePanelIndex { get; set; }

    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        await GetStrategicTraitCount();
    }

    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.Strategy;
    }
    #endregion

    #region Strategic Trait Count
    private GetStrategicTraitCountDto StrategicTraitCountDto { get; set; } = new();

    private async Task GetStrategicTraitCount()
    {
        var result = await StrategicTraitService.GetStrategicTraitCount();

        if (result?.Result is null)
        {
            SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        StrategicTraitCountDto = result.Result;
    }
    #endregion
    
    #region Component Module Update on Count 
    private async Task HandleStrategicTraitCounts()
    {
        await GetStrategicTraitCount();
        
        StateHasChanged();
    }
    #endregion
}