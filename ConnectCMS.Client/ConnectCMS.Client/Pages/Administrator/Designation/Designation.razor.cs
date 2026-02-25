using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Requests.Designation;
using CMSTrain.Client.Models.Responses.Designation;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CMSTrain.Client.Pages.Administrator.Designation;

public partial class Designation : ComponentBase
{
    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        
        await GetAllDesignations();
    }
    
    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.Designation;
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
        
        await GetAllDesignations();
    }

    private bool? IsActive { get; set; } = Constants.ActivationStatus.Active;

    private async Task OnStatusFilter(bool? isActive)
    {
        IsActive = isActive;
        
        PageSize = Constants.Pagination.Size;

        PageNumber = Constants.Pagination.Page;
        
        Designations = null;
        
        await GetAllDesignations();
    }
    #endregion
    
    #region Get All Designation Details
    private CollectionDto<GetDesignationDto>? Designations { get; set; }
    
    private int PageNumber { get; set; } = Constants.Pagination.Page;
    
    private int PageSize { get; set; } = Constants.Pagination.Size; 

    private async Task UpdatePageNumber(int pageNumber)
    {
        PageNumber = pageNumber;
        
        Designations = null;

        await GetAllDesignations(); 
    }

    private async Task UpdatePageSize(int pageSize)
    {
        PageNumber = 1;
        
        PageSize = pageSize;

        Designations = null;
        
        await GetAllDesignations();
    }
    
    private async Task GetAllDesignations()
    {
        try
        {
            var response = await DesignationService.GetAllDesignations(PageNumber, PageSize, Search, IsActive);

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            Designations = response;
        
            StateHasChanged();
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
        
    }
    #endregion
    
    #region Create 
    private bool IsCreateModalOpen { get; set; }

    private CreateDesignationDto CreateDesignationDto { get; set; } = new();
    
    private bool _isCreateButtonDisabled;

    private bool IsCreateDesignationButtonDisabled
    {
        get => _isCreateButtonDisabled ||
               string.IsNullOrEmpty(CreateDesignationDto.Title) ||
               string.IsNullOrEmpty(CreateDesignationDto.Description);
        set => _isCreateButtonDisabled = value;
    }
    
    private void HandleDesignationBusySubmit(bool isBusySubmitting)
    {
        IsCreateDesignationButtonDisabled = isBusySubmitting;
        
        StateHasChanged();
    }

    private void OpenCloseCreateDesignationModal()
    {
        IsCreateModalOpen = !IsCreateModalOpen;

        StateHasChanged();
    }

    private void OpenCreateDesignationModal()
    {
        CreateDesignationDto = new CreateDesignationDto();

        OpenCloseCreateDesignationModal();
    }

    private async Task InsertDesignation(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseCreateDesignationModal();

            return;
        }

        try
        {
            HandleDesignationBusySubmit(true);

            var result = await DesignationService.InsertDesignation(CreateDesignationDto);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);

                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await GetAllDesignations();
                    OpenCloseCreateDesignationModal();
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
            HandleDesignationBusySubmit(false);
        }
    }
    #endregion
    
    #region Update 
    private bool IsEditModalOpen { get; set; }

    private UpdateDesignationDto UpdateDesignationDto { get; set; } = new();
    
    private bool _isUpdateButtonDisabled;
    
    private bool IsUpdateDesignationButtonDisabled
    {
        get => _isUpdateButtonDisabled || 
               string.IsNullOrEmpty(UpdateDesignationDto.Title) ||
               string.IsNullOrEmpty(UpdateDesignationDto.Description);
        set => _isUpdateButtonDisabled = value;
    }
    
    private void HandleDesignationUpdateBusySubmit(bool isBusySubmitting)
    {
        IsUpdateDesignationButtonDisabled = isBusySubmitting;
        
        StateHasChanged();
    }

    private void OpenCloseEditModal()
    {
        IsEditModalOpen = !IsEditModalOpen;

        StateHasChanged();
    }

    private async Task OpenDesignationUpdateModal(Guid designation)
    {
        var response = await DesignationService.GetDesignationById(designation);

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        var designationModel = response.Result;

        UpdateDesignationDto = new UpdateDesignationDto()
        {
            Id = designationModel.Id,
            Title = designationModel.Title,
            Description = designationModel.Description,
        };

        OpenCloseEditModal();
    }

    private async Task UpdateDesignation(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseEditModal();

            return;
        }

        try
        {
            HandleDesignationUpdateBusySubmit(true);

            var result = await DesignationService.UpdateDesignation(UpdateDesignationDto);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    OpenCloseEditModal();
                    await GetAllDesignations();
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
            HandleDesignationUpdateBusySubmit(false);
        }
    }
    #endregion
    
    #region Delete
    private bool IsDeleteModalOpen { get; set; }

    private GetDesignationDto DeleteDesignationDto { get; set; } = new();

    private void OpenCloseDeleteModal()
    {
        IsDeleteModalOpen = !IsDeleteModalOpen;

        StateHasChanged();
    }

    private async Task OpenDesignationDeleteModal(Guid designation)
    {
        var response = await DesignationService.GetDesignationById(designation);

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        DeleteDesignationDto = response.Result;

        OpenCloseDeleteModal();
    }

    private async Task DeleteDesignation(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseDeleteModal();
            return;
        }

        try
        {
            var result = await DesignationService.ActivateDeactivateDesignation(DeleteDesignationDto.Id);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    OpenCloseDeleteModal();
                    await GetAllDesignations();
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
}