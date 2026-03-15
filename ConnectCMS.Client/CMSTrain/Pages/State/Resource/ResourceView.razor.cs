using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Requests.Identity;
using CMSTrain.Client.Models.Responses.Resource;

namespace CMSTrain.Client.Pages.State.Resource;

// Resource Validation - Currently Disabled
// Navigated via the Resource Identifier Parameter instead of the Resource Module Identifier
public partial class ResourceView
{
    [Parameter] public Guid ResourceId { get; set; }

    private bool IsLoading { get; set; } = true;
    
    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        
        var resourceModule = await GetResourceModuleDetails();

        // var role = await GetUserRole();

        // var isResourceAvailable = await GetResourceAvailability(role, resourceModule);

        await AccessResourceMaterial(resourceModule, true);

        ResourceType = resourceModule.Type;

        IsLoading = false;

        // if (isNavigationRequired) NavigateToTrainingDetails(role, resourceModule);
    }

    // Uncomment the Following Code to Enable Copy Protection using JavaScript (Currently Disabled as Not Compatible with All Devices)
    // protected override async Task OnAfterRenderAsync(bool firstRender)
    // {
    //     if (firstRender)
    //     {
    //         await JsRuntime.InvokeVoidAsync("enableCopyProtection");
    //     }
    // }
    
    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.ResourceView;
    }
    #endregion

    #region Resource Materials
    private GetResourceDetailsDto Resource { get; set; } = new();

    private string ResourceType { get; set; } = FileType.Documents.ToString();

    private string ResourceImage { get; set; } = string.Empty;
    #endregion

    #region Resource Module Details
    private async Task<GetResourceDetailsDto> GetResourceModuleDetails()
    {
        try
        {
            var result = await ResourceService.GetResourceById(ResourceId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Warning, Variant.Outlined);

                return new GetResourceModuleDetailsDto();
            }

            var resourceDetails = result.Result;

            Resource = new GetResourceDetailsDto()
            {
                Id = resourceDetails.Id,
                Link = resourceDetails.Link,
                Type = resourceDetails.Type,
                Description = resourceDetails.Description,
                Tag = resourceDetails.Tag,
                IsLink = resourceDetails.IsLink,
                Title = resourceDetails.Title
            };
            
            return result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }

        return new GetResourceModuleDetailsDto();
    }

    #endregion

    #region Roles
    private async Task<RolesDto> GetUserRole()
    {
        try
        {
            var result = await ProfileService.GetUserRole();

            if (result?.Result is not null) return result.Result;

            SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Warning,
                Variant.Outlined);

            return new RolesDto();
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }

        return new RolesDto();
    }

    #endregion

    #region Resource Availability
    // private async Task<bool> GetResourceAvailability(RolesDto role, GetResourceModuleDetailsDto resourceModule)
    // {
    //     try
    //     {
    //         if (role.Name is Constants.Roles.SuperAdmin or Constants.Roles.Admin or Constants.Roles.Trainer)
    //         {
    //             return true;
    //         }
    //
    //         if (Module == Constants.Resource.Training)
    //         {
    //             var result = await ConfigurationService.GetTrainingResourceConfigurationByKey(
    //                 resourceModule.DetailId ?? Guid.Empty,
    //                 TrainingConfiguration.RESOURCE_AVAILABILITY.ToString());
    //
    //             if (result?.Result is null)
    //             {
    //                 SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage,
    //                     Severity.Warning, Variant.Outlined);
    //                 return false;
    //             }
    //
    //             var trainingResponseConfiguration = result.Result.Accessibility;
    //
    //             return DateTime.Now >= trainingResponseConfiguration.AccessPeriod &&
    //                    DateTime.Now <= trainingResponseConfiguration.ExpirePeriod;
    //         }
    //         else
    //         {
    //             var result = await ConfigurationService.GetClassResourceConfigurationByKey(
    //                 resourceModule.DetailId ?? Guid.Empty,
    //                 ClassConfiguration.RESOURCE_AVAILABILITY.ToString());
    //
    //             if (result?.Result is null)
    //             {
    //                 SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage,
    //                     Severity.Warning, Variant.Outlined);
    //                 return false;
    //             }
    //
    //             var classResponseConfiguration = result.Result.Accessibility;
    //
    //             return DateTime.Now >= classResponseConfiguration.AccessPeriod &&
    //                    DateTime.Now <= classResponseConfiguration.ExpirePeriod;
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
    //     }
    //
    //     return false;
    // }

    private async Task<bool> AccessResourceMaterial(GetResourceDetailsDto resourceModule, bool isResourceAvailable)
    {
        try
        {
            if (isResourceAvailable)
            {
                if (resourceModule.Type == FileType.Link.ToString())
                {
                    if (string.IsNullOrEmpty(resourceModule.Link)) return true;

                    var url = resourceModule.Link.StartsWith("https://")
                        ? resourceModule.Link
                        : $"https://{resourceModule.Link}";

                    NavigationManager.NavigateTo(url);

                    return false;
                }

                if (resourceModule.Type == FileType.Image.ToString())
                {
                    var result = await ResourceService.NavigateToResourceMaterialLink(resourceModule.Id);
                    
                    if (result?.Result is null)
                    {
                        SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Warning, Variant.Outlined);

                        return true;
                    }

                    ResourceImage = result.Result;

                    return false;
                }

                if (!resourceModule.IsLink)
                {
                    var result = await ResourceService.DownloadResourceMaterial(resourceModule.Id);

                    if (result?.Result is null)
                    {
                        SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Warning, Variant.Outlined);
                        return true;
                    }

                    switch (result.StatusCode)
                    {
                        case StatusCode.Status200Ok:
                            SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
                            break;
                        case StatusCode.Status404NotFound:
                        case StatusCode.Status400BadRequest:
                        case StatusCode.Status401Unauthorized:
                            SnackbarService.ShowSnackbar(result.Message, Severity.Warning, Variant.Outlined);
                            break;
                        case StatusCode.Status500InternalServerError:
                            SnackbarService.ShowSnackbar(result.Message, Severity.Warning, Variant.Outlined);
                            break;
                    }

                    return true;
                }

                return false;
            }

            SnackbarService.ShowSnackbar("The following resource has either not been configured or is not available due to its time period.", Severity.Warning, Variant.Outlined);
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
        
        return true;
    }
    #endregion

    #region Training Navigation
    // private void NavigateToTrainingDetails(RolesDto role, GetResourceModuleDetailsDto resourceModule)
    // {
    //     if (string.IsNullOrEmpty(role.Name) || resourceModule.DetailId == Guid.Empty || resourceModule.DetailId == null) 
    //         return;
    //
    //     if (Module == Constants.Resource.Training)
    //     {
    //         switch (role.Name)
    //         {
    //             case Constants.Roles.Admin:
    //             case Constants.Roles.SuperAdmin:
    //                 NavigationManager.NavigateTo($"/trainings/admin/training-details/{resourceModule.DetailId}");
    //                 break;
    //             case Constants.Roles.Client:
    //                 NavigationManager.NavigateTo($"/client/assigned-trainings/training-details/{resourceModule.DetailId}");
    //                 break;
    //             case Constants.Roles.Trainer:
    //                 NavigationManager.NavigateTo($"/trainer/assigned-trainings/training-details/{resourceModule.DetailId}");
    //                 break;
    //             case Constants.Roles.Candidate:
    //                 NavigationManager.NavigateTo($"/candidate/assigned-trainings/training-details/{resourceModule.DetailId}");
    //                 break;
    //         }
    //     }
    //     else if (Module == Constants.Resource.Class)
    //     {
    //         switch (role.Name)
    //         {
    //             case Constants.Roles.Admin:
    //             case Constants.Roles.SuperAdmin:
    //                 NavigationManager.NavigateTo($"/trainings/admin-class-details/{resourceModule.DetailId}");
    //                 break;
    //             case Constants.Roles.Client:
    //                 NavigationManager.NavigateTo($"/client/assigned-trainings/client-class-details/{resourceModule.DetailId}");
    //                 break;
    //             case Constants.Roles.Trainer:
    //                 NavigationManager.NavigateTo($"/trainer/assigned-trainings/trainer-class-details/{resourceModule.DetailId}");
    //                 break;
    //             case Constants.Roles.Candidate:
    //                 NavigationManager.NavigateTo($"/candidate/assigned-trainings/candidate-class-details/{resourceModule.DetailId}");
    //                 break;
    //         }
    //     }
    // }
    #endregion
}