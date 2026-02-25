using MudBlazor;
using CMSTrain.Client.Models.Base;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Requests.Heading;
using CMSTrain.Client.Models.Responses.Heading;

namespace CMSTrain.Client.Pages.Administrator.Heading;

public partial class Heading : ComponentBase
{
    protected override async Task OnInitializedAsync()
    {
        await GetAllHeadings();
        await GetAllParentHeadings();
    }

    [Parameter] public HeadingType HeadingType { get; set; }

    [Parameter] public FacetType FacetType { get; set; }

    [Parameter] public InspectionType InspectionType { get; set; }

    [Parameter] public EventCallback? OnHeadingCountUpdate { get; set; }
    
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
        
        await GetAllHeadings();
    }
    
    private bool? IsActive { get; set; } = Constants.ActivationStatus.Active;

    private async Task OnStatusFilter(bool? isActive)
    {
        IsActive = isActive; 
        
        PageSize = Constants.Pagination.Size;

        PageNumber = Constants.Pagination.Page;
        
        Headings = null;

        await GetAllHeadings();
    }
    #endregion

    #region Get All Headings
    private CollectionDto<GetHeadingDto>? Headings { get; set; }
    
    private int PageNumber { get; set; } = Constants.Pagination.Page;
    
    private int PageSize { get; set; } = Constants.Pagination.Size; 

    private async Task UpdatePageNumber(int pageNumber)
    {
        PageNumber = pageNumber;
        
        Headings = null;
        
        await GetAllHeadings();
    }

    private async Task UpdatePageSize(int pageSize)
    {
        PageNumber = 1;
        
        PageSize = pageSize;
        
        Headings = null;
        
        await GetAllHeadings();
    }
    
    private async Task GetAllHeadings()
    {
        try
        {
            var response = await HeadingService.GetAllHeadings(HeadingType, FacetType, InspectionType, PageNumber, PageSize, IsActive, Search);

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            Headings = response;
            
            StateHasChanged();
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion

    #region Get All Parent Headings
    private List<GetHeadingModuleDto> ParentHeadings { get; set; } = [];
    
    private async Task GetAllParentHeadings()
    {
        try
        {
            var response = await HeadingService.GetAllParentHeadings(FacetType, InspectionType);

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            ParentHeadings = response.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion

    #region Insert Headings
    private bool IsCreateModalOpen { get; set; }
    
    private CreateHeadingDto CreateHeadingDto { get; set; } = new();
    
    private bool _isCreateButtonDisabled;

    private bool IsCreateHeadingButtonDisabled
    {
        get => _isCreateButtonDisabled ||
               string.IsNullOrEmpty(CreateHeadingDto.Title) ||
               string.IsNullOrEmpty(CreateHeadingDto.Description) ||
               CreateHeadingDto.Type == HeadingType.None;
        set => _isCreateButtonDisabled = value;
    }
    
    private void HandleHeadingBusySubmit(bool isBusySubmitting)
    {
        IsCreateHeadingButtonDisabled = isBusySubmitting;
        
        StateHasChanged();
    }
    
    private void OpenCloseCreateHeadingModal()
    {
        IsCreateModalOpen = !IsCreateModalOpen;

        StateHasChanged();
    }
    
    private void OpenCreateHeadingModal()
    {
        CreateHeadingDto = new CreateHeadingDto()
        {
            Type = HeadingType,
            Facet = FacetType,
            Inspection = InspectionType
        };

        OpenCloseCreateHeadingModal();
    }
    
    private async Task InsertHeading(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseCreateHeadingModal();
            return;
        }

        try
        {
            HandleHeadingBusySubmit(true);

            var result = await HeadingService.InsertHeading(CreateHeadingDto);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await GetAllHeadings();
                    await GetAllParentHeadings();
                    OpenCloseCreateHeadingModal();
                    await HandleHeadingsCountUpdate();
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
            HandleHeadingBusySubmit(false);
        }
    }

    private bool IsCreateHeadingDisabled =>
        string.IsNullOrWhiteSpace(CreateHeadingDto.Title) || 
        string.IsNullOrWhiteSpace(CreateHeadingDto.Description) || 
        CreateHeadingDto.Type == HeadingType.None || 
        (CreateHeadingDto.Type == HeadingType.SubHeading && CreateHeadingDto.ParentHeadingId == Guid.Empty);

    private async Task HandleHeadingsCountUpdate()
    {
        if (OnHeadingCountUpdate != null)
        {
            await OnHeadingCountUpdate.Value.InvokeAsync();
        }
    }
    #endregion

    #region Active Deactive Header 
    private bool IsDeleteModalOpen { get; set; }
    
    private GetHeadingDto DeleteHeadingDto { get; set; } = new();

    private void OpenCloseDeleteModal()
    {
        IsDeleteModalOpen = !IsDeleteModalOpen;

        StateHasChanged();
    }

    private async Task OpenHeadingDeleteModal(Guid headingId)
    {
        var response = await HeadingService.GetHeadingById(headingId);

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        DeleteHeadingDto = response.Result;

        OpenCloseDeleteModal();
    }

    private async Task DeleteHeading(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseDeleteModal();
            return;
        }

        try
        {
            var result = await HeadingService.ActivateDeactivateHeading(DeleteHeadingDto.Id);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    OpenCloseDeleteModal();
                    await GetAllHeadings();
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

    #region Update
    private GetHeadingDto GetHeading { get; set; } = new();

    private bool IsEditModalOpen { get; set; }

    private UpdateHeadingDto UpdateHeadingDto { get; set; } = new();
    
    private bool _isUpdateButtonDisabled;
    
    private bool IsUpdateHeadingButtonDisabled
    {
        get => _isUpdateButtonDisabled || 
               string.IsNullOrEmpty(UpdateHeadingDto.Title) ||
               string.IsNullOrEmpty(UpdateHeadingDto.Description) ||
               UpdateHeadingDto.Type == HeadingType.None;
        set => _isUpdateButtonDisabled = value;
    }
    
    private void HandleHeadingUpdateBusySubmit(bool isBusySubmitting)
    {
        IsUpdateHeadingButtonDisabled = isBusySubmitting;
        
        StateHasChanged();
    }

    private void OpenCloseEditModal()
    {
        IsEditModalOpen = !IsEditModalOpen;

        StateHasChanged();
    }

    private async Task OpenHeadingUpdateModal(Guid headingId)
    {
        var response = await HeadingService.GetHeadingById(headingId);

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        GetHeading = response.Result;

        UpdateHeadingDto = new UpdateHeadingDto()
        {
            Id = GetHeading.Id,
            Title = GetHeading.Title,
            Description = GetHeading.Description,
        };

        OpenCloseEditModal();
    }

    private async Task UpdateHeading(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseEditModal();

            return;
        }

        try
        {
            HandleHeadingUpdateBusySubmit(true);

            var result = await HeadingService.UpdateHeading(UpdateHeadingDto);

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
                    await GetAllHeadings();
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
            HandleHeadingUpdateBusySubmit(false);
        }
    }
    #endregion

    #region Sub Headings
    private List<GetHeadingModuleDto> SubHeading { get; set; } = [];

    private async Task GetSubHeadingDetails()
    {
        try
        {
            var response = await HeadingService.GetAllSubHeadings();

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);

                return;
            }

            SubHeading = response.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }

    private bool IsHeadingDetailsModalOpen { get; set; }

    private GetHeadingDto OpenedHeadingDetails { get; set; } = new();

    private void OpenCloseHeadingDetailsModal()
    {
        IsHeadingDetailsModalOpen = !IsHeadingDetailsModalOpen;

        if (!IsHeadingDetailsModalOpen)
        {
            OpenedHeadingDetails = new GetHeadingDto();
        }

        StateHasChanged();
    }

    private void OpenAllSubHeadingDetailsModal(GetHeadingDto heading)
    {
        OpenedHeadingDetails = heading;

        OpenCloseHeadingDetailsModal();
    }
    #endregion 
}