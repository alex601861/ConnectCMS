using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Requests.Resource;

namespace CMSTrain.Client.Pages.Administrator.Resource;

public partial class ResourceUpdate
{
    [Parameter] public Guid ResourceId { get; set; }
    
    private ResourcePostUpdateDto Resource { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();

        await GetResourcePostDetails();
    }
    
    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.ResourcePost;
    }
    #endregion

    #region Resource Details
    private async Task GetResourcePostDetails()
    {
        try
        {
            var result = await ResourceService.GetResourceById(ResourceId);
            
            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            var resource = result.Result;
            
            Resource = new ResourcePostUpdateDto()
            {
                Id = resource.Id,
                Title = resource.Title,
                Description = resource.Description,
                IsLink = resource.IsLink,
                Link = resource.Link,
                Tag = resource.Tag ?? ""
            };
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    

    #endregion

    #region Update Resource Post
    private async Task UpdateResourcePost()
    {
        try
        {
            var result = await ResourceService.UpdateResource(Resource);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
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
    }
    #endregion
}