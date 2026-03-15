using MudBlazor;
using Microsoft.JSInterop;
using CMSTrain.Client.Models.Base;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using Microsoft.AspNetCore.Components.Forms;
using CMSTrain.Client.Models.Requests.Resource;
using CMSTrain.Client.Models.Responses.Resource;
using CMSTrain.Client.Models.Requests.Configuration.Class;
using CMSTrain.Client.Models.Requests.Configuration.Training;

namespace CMSTrain.Client.Pages.State.Resource;

public partial class ResourceDetails : ComponentBase
{
    [Parameter] public Guid ModuleId { get; set; }

    [Parameter] public bool IsMaterialForTraining { get; set; }

    [Parameter] public bool IsAccessedByAdmin { get; set; }

    [Parameter] public bool IsEditable { get; set; }

    [Parameter] public bool IsDeletable { get; set; }

    [Parameter] public EventCallback OnResourceMaterialsCountUpdate { get; set; }

    private bool IsResourceMaterialAccessible { get; set; }

    private IReadOnlyCollection<Guid> ResourceMaterialIds { get; set; } = [];

    private List<GetResourceDetailsDto> ResourceDetail { get; set; } = [];

    private bool IsDisplayedAsGrid { get; set; } = true;
    
    protected override async Task OnInitializedAsync()
    {
        if (IsEditable)
        {
            await GetResourceMaterials();
            
            await GetResourceModuleMaterials();
            
            IsResourceMaterialAccessible = true;    
        }
        else
        {
            await GetResourceAvailability();
        }
    }

    #region Display Format
    private void HandleDisplayFormatChange(bool value)
    {
        IsDisplayedAsGrid = value;
        
        StateHasChanged();
    }
    #endregion
    
    #region Search
    private string _search = string.Empty;
    
    private string Search
    {
        get => _search;
        set
        {
            if (_search == value) return;
            _search = value;
            _ = OnSearchInputAsync(_search);
        }
    }
    
    private async Task OnSearchInputAsync(string search)
    {
        Search = search;
        
        PageSize = Constants.Pagination.Size;
        
        PageNumber = Constants.Pagination.Page;
        
        await GetResourceModuleMaterials();
        
        StateHasChanged();
    }
    #endregion
    
    #region Resource Materials
    private async Task GetResourceMaterials()
    {
        try
        {
            var result = await ResourceService.GetAllResources();

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            ResourceDetail = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    
    private static string GetResourceMaterialName(GetResourceDetailsDto resource)
    {
        return $"{resource.Title} ({resource.Type})";
    }
    #endregion
    
    #region Module Resource Materials
    private CollectionDto<GetResourceModuleDetailsDto>? ModuleResourceDetail { get; set; }
    
    private int PageNumber { get; set; } = Constants.Pagination.Page;
    
    private int PageSize { get; set; } = Constants.Pagination.Size; 
    
    private async Task UpdatePageNumber(int pageNumber)
    {
        PageNumber = pageNumber;
        
        ModuleResourceDetail = null;
        
        await GetResourceModuleMaterials();
    }

    private async Task UpdatePageSize(int pageSize)
    {
        PageNumber = 1;
        
        PageSize = pageSize;
        
        ModuleResourceDetail = null;
        
        await GetResourceModuleMaterials();
    }

    private async Task GetResourceModuleMaterials()
    {
        try
        {
            var result = IsMaterialForTraining 
                ? await ResourceService.GetAllResourcesForTraining(ModuleId, PageNumber, PageSize, Search, IsEditable ? null : true)
                : await ResourceService.GetAllResourcesForClass(ModuleId, PageNumber, PageSize, Search, IsEditable ? null : true);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            ModuleResourceDetail = result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    
    private async Task GetResourceModuleMaterialsDetails()
    {
        try
        {
            var result = IsMaterialForTraining 
                ? await ResourceService.GetAllResourcesForTraining(ModuleId)
                : await ResourceService.GetAllResourcesForClass(ModuleId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            ResourceMaterialIds = result.Result.Select(x => x.Id).ToList();
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion

    #region Resource Availibility
    private bool IsResourceConfigurationModalOpen { get; set; }
    
    private async Task  OpenCloseResourceConfigurationModal()
    {
        IsResourceConfigurationModalOpen = !IsResourceConfigurationModalOpen;

        await GetResourceConfiguration();
        
        StateHasChanged();
    }
    
    private TrainingResourceConfiguration TrainingResourceConfiguration { get; set; } = new();
    
    private ClassResourceConfiguration ClassResourceConfiguration { get; set; } = new();
    
    private async Task GetResourceConfiguration()
    {
        try
        {
            if (IsMaterialForTraining)
            {
                var result = await ConfigurationService.GetTrainingResourceConfigurationByKey(ModuleId,
                    TrainingConfiguration.RESOURCE_AVAILABILITY.ToString());
                
                if (result?.Result is null)
                {
                    SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                    return;
                }

                TrainingResourceConfiguration = result.Result;
            }
            else
            {
                var result = await ConfigurationService.GetClassResourceConfigurationByKey(ModuleId, 
                    ClassConfiguration.RESOURCE_AVAILABILITY.ToString());
                
                if (result?.Result is null)
                {
                    SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                    return;
                }

                ClassResourceConfiguration = result.Result;
            }
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }

    private async Task UploadResourceAvailabilityConfigurations(bool isClosed)
    {
        if (isClosed)
        {
            await OpenCloseResourceConfigurationModal();
            return;
        }

        try
        {
            var result = IsMaterialForTraining
                ? await ConfigurationService.SaveTrainingResourceConfiguration(ModuleId,
                    TrainingConfiguration.RESOURCE_AVAILABILITY.ToString(), TrainingResourceConfiguration)
                : await ConfigurationService.SaveClassResourceConfiguration(ModuleId,
                    ClassConfiguration.RESOURCE_AVAILABILITY.ToString(), ClassResourceConfiguration);
            
            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }
            
            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await OpenCloseResourceConfigurationModal();
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
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
    
    private async Task GetResourceAvailability()
    {
        try
        {
            if (IsMaterialForTraining)
            {
                var result = await ConfigurationService.GetTrainingResourceConfigurationByKey(ModuleId,
                    TrainingConfiguration.RESOURCE_AVAILABILITY.ToString());
                
                if (result?.Result is null)
                {
                    SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                    return;
                }

                var trainingResponseConfiguration = result.Result.Accessibility;

                if (DateTime.Now >= trainingResponseConfiguration.AccessPeriod && DateTime.Now <= trainingResponseConfiguration.ExpirePeriod)
                {
                    IsResourceMaterialAccessible = true;
                    
                    await GetResourceModuleMaterials();
                }
                else
                {
                    IsResourceMaterialAccessible = false;

                    ModuleResourceDetail = new CollectionDto<GetResourceModuleDetailsDto>();
                }
            }
            else
            {
                var result = await ConfigurationService.GetClassResourceConfigurationByKey(ModuleId, 
                    ClassConfiguration.RESOURCE_AVAILABILITY.ToString());
                
                if (result?.Result is null)
                {
                    SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                    return;
                }
                
                var classResponseConfiguration = result.Result.Accessibility;

                if (DateTime.Now >= classResponseConfiguration.AccessPeriod && DateTime.Now <= classResponseConfiguration.ExpirePeriod)
                {
                    IsResourceMaterialAccessible = true;

                    await GetResourceModuleMaterials();
                }
                else
                {
                    IsResourceMaterialAccessible = false;

                    ModuleResourceDetail = new CollectionDto<GetResourceModuleDetailsDto>();
                }
            }
            
            StateHasChanged();
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion

    #region Upload Resource via Entity
    private bool IsResourceMaterialUploadModalOpen { get; set; }
    
    private TrainingResourceUploadDto TrainingResourceUpload { get; set; } = new();

    private ClassResourceUploadDto ClassResourceUpload { get; set; } = new();

    private async Task OpenResourceMaterialUploadModal()
    {
        OpenCloseResourceMaterialUploadModal();

        if (IsMaterialForTraining)
        {
            TrainingResourceUpload = new TrainingResourceUploadDto
            {
                TrainingId = ModuleId
            };
        }
        else
        {
            ClassResourceUpload = new ClassResourceUploadDto
            {
                ClassId = ModuleId
            };
        }

        await GetResourceModuleMaterialsDetails();
    }

    private async Task UploadResourceMaterials(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseResourceMaterialUploadModal();
            return;
        }

        try
        {
            if (IsMaterialForTraining)
            {
                TrainingResourceUpload.ResourceIds = ResourceMaterialIds.ToList();
            }
            else
            {
                ClassResourceUpload.ResourceIds = ResourceMaterialIds.ToList();
            }
            
            var result = IsMaterialForTraining 
                ? await ResourceService.UploadResourcesForTraining(TrainingResourceUpload)
                : await ResourceService.UploadResourcesForClass(ClassResourceUpload);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    OpenCloseResourceMaterialUploadModal();
                    await GetResourceModuleMaterials();
                    await OnResourceMaterialsCountUpdate.InvokeAsync();
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
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

    private void OpenCloseResourceMaterialUploadModal()
    {
        IsResourceMaterialUploadModalOpen = !IsResourceMaterialUploadModalOpen;
        
        StateHasChanged();
    }
    #endregion

    #region Upload Resource via Link
    private bool IsResourceModuleUploadModalOpen { get; set; }
    
    private ResourceModuleUploadDto ResourceModuleUpload { get; set; } = new();

    private async Task OpenResourceModuleUploadModal()
    {
        OpenCloseResourceModuleUploadModal();

        ResourceModuleUpload = new ResourceModuleUploadDto
        {
            ModuleId = ModuleId,
            IsMaterialForTraining = IsMaterialForTraining
        };
        
        await GetResourceModuleMaterialsDetails();
    }

    private async Task UploadResourceModuleMaterials(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseResourceModuleUploadModal();
            
            return;
        }

        try
        {
            var result = await ResourceService.UploadResourceModule(ResourceModuleUpload);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await GetResourceModuleMaterials();
                    OpenCloseResourceModuleUploadModal();
                    await OnResourceMaterialsCountUpdate.InvokeAsync();
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
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

    private void OpenCloseResourceModuleUploadModal()
    {
        IsResourceModuleUploadModalOpen = !IsResourceModuleUploadModalOpen;
        
        StateHasChanged();
    }

    private void OnHandleResourceUpload(IBrowserFile? resourceMaterial)
    {
        ResourceModuleUpload.Resource.ResourceFile = resourceMaterial;
    }
    
    #endregion
    
    #region Remove Resource Materials
    private bool IsResourceDeleteModalOpen { get; set; }
    
    private GetResourceDetailsDto DeleteResourceDetails { get; set; } = new();

    private void OpenResourceDeleteModal(Guid moduleId)
    {
        OpenCloseResourceDeleteModal();

        DeleteResourceDetails = new GetResourceDetailsDto()
        {
            Id = moduleId
        };
    }

    private async Task DeleteResourceMaterial(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseResourceDeleteModal();
            return;
        }

        try
        {
            var result = IsMaterialForTraining 
                ? await ResourceService.RemoveResourceMaterialFromTraining(DeleteResourceDetails.Id)
                : await ResourceService.RemoveResourceMaterialFromClass(DeleteResourceDetails.Id);

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
                    await GetResourceModuleMaterials();
                    OpenCloseResourceDeleteModal();
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

    private void OpenCloseResourceDeleteModal()
    {
        IsResourceDeleteModalOpen = !IsResourceDeleteModalOpen;
        StateHasChanged();
    }
    #endregion
    
    #region Download Resource Material
    private async Task DownloadResourceMaterial(Guid resourceId)
    {
        try
        {
            var result = await ResourceService.DownloadResourceMaterial(resourceId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
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

    #region Resource Post Navigation
    private void NavigateToResourceDetails(Guid resourceId)
    {
        NavigationManager.NavigateTo($"resource/view/{resourceId}");
    }
    
    private async Task NavigateToResourceLink(GetResourceModuleDetailsDto resourceModuleDetails)
    {
        var link = resourceModuleDetails.Link!.StartsWith("http") ? resourceModuleDetails.Link : $"https://{resourceModuleDetails.Link}";
        
        await JsRuntime.InvokeVoidAsync("openInNewTab", link);
    }
    #endregion

    #region Activate Deactive Resources
    private GetResourceModuleDetailsDto ActivateDeactivateResource { get; set; } = new();
    
    private bool IsActivateDeactivateResourceModalOpen { get; set; }
    
    private void OpenCloseActivateDeactivateResourceModal(Guid resourceModuleId, bool isActive)
    {
        IsActivateDeactivateResourceModalOpen = !IsActivateDeactivateResourceModalOpen;

        if (IsActivateDeactivateResourceModalOpen)
        {
            ActivateDeactivateResource = new GetResourceModuleDetailsDto
            {
                ModuleId = resourceModuleId,
                IsActive = isActive
            };
        }
        else
        {
            ActivateDeactivateResource = new GetResourceModuleDetailsDto();
        }
        
        StateHasChanged();
    }
    
    private async Task ActivateDeactivateResourceMaterial(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseActivateDeactivateResourceModal(ActivateDeactivateResource.ModuleId, ActivateDeactivateResource.IsActive);
            return; 
        }
        
        try
        {
            var result = IsMaterialForTraining 
                ? await ResourceService.ActivateDeactivateResourceForTraining(ActivateDeactivateResource.ModuleId)
                : await ResourceService.ActivateDeactivateResourceForClass(ActivateDeactivateResource.ModuleId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await GetResourceModuleMaterials();
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
                    OpenCloseActivateDeactivateResourceModal(ActivateDeactivateResource.ModuleId, ActivateDeactivateResource.IsActive);
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
    
    #region Generate and Download QR Code
    private bool IsGenerateQrCodeModalOpen { get; set; }

    private string QrCodeImage { get; set; } = string.Empty;
    
    private ResourceGenerateQrCodeDto ResourceGenerateQrCode { get; set; } = new();
    
    private async Task OpenCloseGenerateQrCodeModal(Guid resourceId)
    {
        IsGenerateQrCodeModalOpen = !IsGenerateQrCodeModalOpen;

        if (IsGenerateQrCodeModalOpen)
        {
            ResourceGenerateQrCode = new()
            {
                ResourceId = resourceId
            };

            await GenerateResourceModuleQrCode();
        }
        else
        {
            QrCodeImage = string.Empty;
            
            ResourceGenerateQrCode = new ResourceGenerateQrCodeDto();
        }
        
        StateHasChanged();
    }

    private async Task GenerateResourceModuleQrCode()
    {
        try
        {
            var result = await ResourceService.GenerateModuleResourceMaterialQrCode(ResourceGenerateQrCode.ResourceId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    if (result.Result.Length == 0)
                    {
                        QrCodeImage = string.Empty;
                        SnackbarService.ShowSnackbar("QR Code of the respective resource material could not be downloaded.", Severity.Warning, Variant.Outlined);
                    }
                    else
                    {
                        var base64Image = Convert.ToBase64String(result.Result);
                        QrCodeImage = $"data:image/png;base64,{base64Image}";
                    }
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
    
    private async Task DownloadModuleResourceMaterialQrCode(bool isClosed)
    {
        if (isClosed)
        {
            await OpenCloseGenerateQrCodeModal(ResourceGenerateQrCode.ResourceId);
            
            return;
        }
        
        try
        {
            var result = await ResourceService.DownloadModuleResourceMaterialQrCode(ResourceGenerateQrCode);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
                    await OpenCloseGenerateQrCodeModal(ResourceGenerateQrCode.ResourceId);
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