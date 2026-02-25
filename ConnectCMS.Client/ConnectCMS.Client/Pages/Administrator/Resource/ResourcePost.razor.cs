using MudBlazor;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Requests.Resource;
using CMSTrain.Client.Layout.Application;
using Microsoft.AspNetCore.Components;

namespace CMSTrain.Client.Pages.Administrator.Resource;

public partial class ResourcePost
{
    private ResourcePostDto Resource { get; set; } = new();

    protected override void OnInitialized()
    {
        SetPageTitle();
    }

    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.ResourcePost;
    }
    #endregion
    
    private bool _isCreateButtonDisabled;

    private bool IsCreateResourcePostButtonDisabled
    {
        get => _isCreateButtonDisabled ||
               string.IsNullOrEmpty(Resource.Title) ||
               string.IsNullOrEmpty(Resource.Description) ||
               string.IsNullOrEmpty(Resource.Tag);
        set => _isCreateButtonDisabled = value;
    }
    
    private void HandleResourcePostBusySubmit(bool isBusySubmitting)
    {
        IsCreateResourcePostButtonDisabled = isBusySubmitting;
        
        StateHasChanged();
    }

    private async Task UploadResourcePost()
    {
        try
        {
            HandleResourcePostBusySubmit(true);

            var result = await ResourceService.UploadResourcesPost(Resource);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
                    NavigationManager.NavigateTo("resources");
                    break;
                case StatusCode.Status404NotFound:
                case StatusCode.Status400BadRequest:
                case StatusCode.Status401Unauthorized:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Warning, Variant.Outlined);
                    break;
                case StatusCode.Status500InternalServerError:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Error, Variant.Outlined);
                    break;
            }
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
        finally
        {
            HandleResourcePostBusySubmit(false);
        }
    }
}