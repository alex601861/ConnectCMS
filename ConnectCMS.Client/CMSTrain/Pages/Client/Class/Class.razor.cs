using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.Class;
using CMSTrain.Application.DTOs.Count;
using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Responses.Count;

namespace CMSTrain.Client.Pages.Client.Class;

public partial class Class : ComponentBase
{
    [Parameter] public Guid ClassId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        await GetAllClassDetails();
        await GetAllCountForClient();
    }

    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.Class;
    }
    #endregion

    #region Panel Navigation
    private int ActivePanelIndex { get; set; }
    #endregion
    
    #region Class Details
    private GetClassDto ClassDetails { get; set; } = new();

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

    #endregion

    #region Get All Class Count for Client

    private ClassCountDto ClassCount { get; set; } = new();

    private async Task GetAllCountForClient()
    {
        try
        {
            var result = await ClassService.GetClassDetailsCountForClient(ClassId);

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
}