using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.Resource;
using CMSTrain.Client.Layout.Application;

namespace CMSTrain.Client.Pages.State.Resource;

public partial class ResourcePost
{
    [Parameter] public Guid ResourceId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        await GetResourceDetails();
    }

    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.ResourceView;
    }
    #endregion

    private GetResourceDetailsDto Resource { get; set; } = new();
    
    private async Task GetResourceDetails()
    {
        try
        {
            var result = await ResourceService.GetResourceById(ResourceId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            Resource = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
}