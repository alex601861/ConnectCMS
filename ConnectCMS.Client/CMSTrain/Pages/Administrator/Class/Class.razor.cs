using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Responses.Class;
using CMSTrain.Client.Models.Responses.Count;

namespace CMSTrain.Client.Pages.Administrator.Class;

public partial class Class : ComponentBase
{
    [Parameter] public Guid ClassId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        await GetAllClassDetails();
        await GetClassCountDetails();
    }

    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.Class;
    }
    #endregion
    
    #region Class Details
    private GetClassDto ClassDetails { get; set; } = new();

    private ClassCountDto ClassCount { get; set; } = new();
    
    private async Task GetAllClassDetails()
    {
        try
        {
            var result = await ClassService.GetClassById(ClassId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }
            
            ClassDetails = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }

    private async Task GetClassCountDetails()
    {
        try
        {
            var result = await ClassService.GetClassCount(ClassId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            ClassCount = result.Result;

        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion
    
    #region Component Module Update on Count 
    private async Task HandleClassDetailsCounts()
    {
        await GetClassCountDetails();
        
        StateHasChanged();
    }
    #endregion

    #region Panel Navigation
    private int ActivePanelIndex { get; set; }
    #endregion
}