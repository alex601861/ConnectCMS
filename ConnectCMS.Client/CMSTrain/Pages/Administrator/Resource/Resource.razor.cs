using MudBlazor;
using CMSTrain.Client.Models.Base;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Layout.Application;
using Microsoft.AspNetCore.Components.Forms;
using CMSTrain.Client.Models.Requests.Resource;
using CMSTrain.Client.Models.Responses.Resource;

namespace CMSTrain.Client.Pages.Administrator.Resource;

public partial class Resource
{
    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();

        await GetAllResources();
    }

    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.Resource;
    }
    #endregion

    #region Search and Filter
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
        
        await GetAllResources();
        
        StateHasChanged();
    }
    #endregion

    #region Resource Details
    private CollectionDto<GetResourceDetailsDto>? ResourceDetail { get; set; }
    
    private int PageNumber { get; set; } = Constants.Pagination.Page;
    
    private int PageSize { get; set; } = Constants.Pagination.Size; 

    private async Task UpdatePageNumber(int pageNumber)
    {
        PageNumber = pageNumber;
        
        ResourceDetail = null;
        
        await GetAllResources();
    }

    private async Task UpdatePageSize(int pageSize)
    {
        PageNumber = 1;
        
        PageSize = pageSize;
        
        ResourceDetail = null;
        
        await GetAllResources();
    }
    
    private async Task GetAllResources()
    {
        try
        {
            var response = await ResourceService.GetAllResources(PageNumber, PageSize, Search);

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            ResourceDetail = response;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion

    #region Resource Post
    private void NavigateToResourcePost(Guid? resourceId = null)
    {
        var url = resourceId != null ? $"resource/post/{resourceId}" : "resources/post";
        
        NavigationManager.NavigateTo(url);
    }
    #endregion
    
    #region Upload Resource
    private bool IsResourceUploadModalOpen { get; set; }
    
    private ResourceUploadDto ResourceUpload { get; set; } = new();
    
    private bool _isCreateButtonDisabled;

    private bool IsCreateResourceButtonDisabled
    {
        get => _isCreateButtonDisabled ||
               string.IsNullOrEmpty(ResourceUpload.Title) ||
               string.IsNullOrEmpty(ResourceUpload.Description) ||
               ResourceUpload is { IsLink: false, ResourceFile: null } ||
               (ResourceUpload.IsLink && string.IsNullOrEmpty(ResourceUpload.Link));
        set => _isCreateButtonDisabled = value;
    }
    
    private void HandleResourceBusySubmit(bool isBusySubmitting)
    {
        IsCreateResourceButtonDisabled = isBusySubmitting;
        
        StateHasChanged();
    }

    private void OpenResourceUploadModal()
    {
        OpenCloseResourceUploadModal();

        ResourceUpload = new ResourceUploadDto();
    }

    private async Task UploadResourceMaterial(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseResourceUploadModal();
            
            return;
        }

        try
        {
            HandleResourceBusySubmit(true);

            var result = await ResourceService.UploadResources(ResourceUpload);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await GetAllResources();
                    OpenCloseResourceUploadModal();
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
        finally
        {
            HandleResourceBusySubmit(false);
        }
    }

    private void OpenCloseResourceUploadModal()
    {
        IsResourceUploadModalOpen = !IsResourceUploadModalOpen;
        
        StateHasChanged();
    }

    private void OnHandleResourceUpload(IBrowserFile? file)
    {
        ResourceUpload.ResourceFile = file;
    }
    #endregion

    #region Delete Resource
    private bool IsResourceDeleteModalOpen { get; set; }
    
    private GetResourceDetailsDto DeleteResourceDetails { get; set; } = new();

    private void OpenResourceDeleteModal(Guid resourceId)
    {
        OpenCloseResourceDeleteModal();

        DeleteResourceDetails = new GetResourceDetailsDto()
        {
            Id = resourceId
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
            var result = await ResourceService.DeleteResourceMaterial(DeleteResourceDetails.Id);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await GetAllResources();
                    OpenCloseResourceDeleteModal();
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

    #region Edit Resource
    private GetResourceDetailsDto UpdateResourceDetailsDto { get; set; } = new();
    
    private bool IsEditModalOpen { get; set; }
    
    private UpdateResourceDto UpdateResourceDto { get; set; } = new();
    
    private bool _isUpdateButtonDisabled;
    
    private bool IsUpdateResourceButtonDisabled
    {
        get => _isUpdateButtonDisabled || 
               string.IsNullOrEmpty(UpdateResourceDto.Title) ||
               string.IsNullOrEmpty(UpdateResourceDto.Description);
        set => _isUpdateButtonDisabled = value;
    }
    
    private void HandleResourceUpdateBusySubmit(bool isBusySubmitting)
    {
        IsUpdateResourceButtonDisabled = isBusySubmitting;
        
        StateHasChanged();
    }
    
    private async Task OpenResourceUpdateModal(Guid resourceId, string? tag)
    {
        if (!string.IsNullOrEmpty(tag))
        {
            NavigationManager.NavigateTo($"/resources/update/{resourceId}");
            
            return;
        }
        
        var response = await ResourceService.GetResourceById(resourceId);
        
        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }
        
        UpdateResourceDetailsDto = response.Result;
        
        UpdateResourceDto = new UpdateResourceDto()
        {
           Id = UpdateResourceDetailsDto.Id,
           Title = UpdateResourceDetailsDto.Title,
           Description = UpdateResourceDetailsDto.Description,
           Link = UpdateResourceDetailsDto.Link,
           IsLink = UpdateResourceDetailsDto.IsLink,
        };
    
        IsEditModalOpen = true;
        
        StateHasChanged();
    }
    
    private async Task UpdateResourceMaterial(bool isClosed)
    {
        if (isClosed)
        {
            IsEditModalOpen = false;
            
            return;
        }

        try
        {
            HandleResourceUpdateBusySubmit(true);

            var result = await ResourceService.UpdateResource(UpdateResourceDto);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await GetAllResources();
                    IsEditModalOpen = false;
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
        finally
        {
            HandleResourceUpdateBusySubmit(false);
        }
    }
    
    private void OnHandleResourceUpdateUpload(IBrowserFile? file)
    {
        UpdateResourceDto.ResourceFile = file;
    }
    #endregion
}