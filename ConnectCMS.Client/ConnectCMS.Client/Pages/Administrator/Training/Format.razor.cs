using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Base;
using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Requests.TrainingFormat;
using CMSTrain.Client.Models.Responses.TrainingFormat;

namespace CMSTrain.Client.Pages.Administrator.Training;

public partial class Format : ComponentBase
{
    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        await GetTrainingFormats();
    }
    
    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.TrainingFormat;
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
        
        await GetTrainingFormats();
        
        StateHasChanged();
    }
    
    private bool? IsActive { get; set; } = Constants.ActivationStatus.Active;

    private async Task OnStatusFilter(bool? isActive)
    {
        IsActive = isActive;
        
        PageSize = Constants.Pagination.Size;

        PageNumber = Constants.Pagination.Page;
        
        TrainingFormats = null;

        await GetTrainingFormats();
    }
    #endregion
    
    #region Training Formats
    private CollectionDto<GetTrainingFormatDto>? TrainingFormats { get; set; }
    
    private int PageNumber { get; set; } = Constants.Pagination.Page;
    
    private int PageSize { get; set; } = Constants.Pagination.Size; 
    
    private async Task UpdatePageNumber(int pageNumber)
    {
        PageNumber = pageNumber;
        
        TrainingFormats = null;
        
        await GetTrainingFormats();
    }

    private async Task UpdatePageSize(int pageSize)
    {
        PageNumber = 1;
        
        PageSize = pageSize;
        
        TrainingFormats = null;
        
        await GetTrainingFormats();
    }

    private async Task GetTrainingFormats()
    {
        try
        {
            var response = await TrainingFormatService.GetTrainingFormats(PageNumber, PageSize, Search, IsActive);

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            TrainingFormats = response;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion

    #region Create
    private bool IsCreateModalOpen { get; set; }

    private CreateTrainingFormatDto CreateTrainingFormatDto { get; set; } = new();

    private void OpenCreateTrainingFormatModal()
    {
        CreateTrainingFormatDto = new CreateTrainingFormatDto();
        IsCreateModalOpen = true;
        StateHasChanged();
    }
    
    private bool _isCreateButtonDisabled;

    private bool IsCreateButtonDisabled
    {
        get => _isCreateButtonDisabled || 
               string.IsNullOrEmpty(CreateTrainingFormatDto.Name) ||
               string.IsNullOrEmpty(CreateTrainingFormatDto.Description);
        set => _isCreateButtonDisabled = value;
    }
    
    private void HandleCreateBusySubmit(bool isBusySubmitting)
    {
        IsCreateButtonDisabled = isBusySubmitting;
        
        StateHasChanged();
    }
    
    private async Task InsertTrainingFormat(bool isClosed)
    {
        if (isClosed)
        {
            IsCreateModalOpen = false;
            return;
        }

        try
        {
            HandleCreateBusySubmit(true);

            var result = await TrainingFormatService.InsertTrainingFormat(CreateTrainingFormatDto);
            
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
                    await GetTrainingFormats();
                    StateHasChanged();
                    IsCreateModalOpen = false;
                    break;
                case StatusCode.Status401Unauthorized:
                case StatusCode.Status400BadRequest:
                case StatusCode.Status404NotFound:
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
            HandleCreateBusySubmit(false);
        }
    }
    #endregion

    #region Edit
    private GetTrainingFormatDto TrainingFormatDto { get; set; } = new();

    private bool IsEditModalOpen { get; set; }

    private UpdateTrainingFormatDto UpdateTrainingFormatDto { get; set; } = new();
    
    private bool _isUpdateButtonDisabled;
    
    private bool IsUpdateButtonDisabled
    {
        get => _isUpdateButtonDisabled || 
               string.IsNullOrEmpty(UpdateTrainingFormatDto.Name) ||
               string.IsNullOrEmpty(UpdateTrainingFormatDto.Description);
        set => _isUpdateButtonDisabled = value;
    }
    
    private void HandleUpdateBusySubmit(bool isBusySubmitting)
    {
        IsUpdateButtonDisabled = isBusySubmitting;
        
        StateHasChanged();
    }

    private async Task OpenTrainingFormatUpdateModal(Guid trainingFormatId)
    {
        var response = await TrainingFormatService.GetTrainingFormatById(trainingFormatId);

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        TrainingFormatDto = response.Result;

        UpdateTrainingFormatDto = new UpdateTrainingFormatDto()
        {
            Id = TrainingFormatDto.Id,
            Name = TrainingFormatDto.Name,
            Description = TrainingFormatDto.Description
        };

        IsEditModalOpen = true;
        
        StateHasChanged();
    }
    
    private async Task UpdateTrainingFormat(bool isClosed)
    {
        if (isClosed)
        {
            IsEditModalOpen = false;
            return;
        }

        try
        {
            HandleUpdateBusySubmit(true);

            var result = await TrainingFormatService.UpdateTrainingFormat(UpdateTrainingFormatDto);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await GetTrainingFormats();
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
                    IsEditModalOpen = false;
                    break;
                case StatusCode.Status401Unauthorized:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Warning, Variant.Outlined);
                    break;
                case StatusCode.Status400BadRequest:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Warning, Variant.Outlined);
                    break;
                case StatusCode.Status404NotFound:
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
            HandleUpdateBusySubmit(false);
        }
    }
    #endregion

    #region Delete
    private bool IsDeleteModalOpen { get; set; }
    
    private GetTrainingFormatDto DeleteTrainingFormatDto { get; set; } = new();

    private async Task OpenTrainingFormatDeleteModal(Guid trainingFormatId)
    {
        var response = await TrainingFormatService.GetTrainingFormatById(trainingFormatId);

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        DeleteTrainingFormatDto = response.Result;

        IsDeleteModalOpen = true;
        
        StateHasChanged();
    }
    
    private async Task DeleteTrainingFormat(bool isClosed)
    {
        if (isClosed)
        {
            IsDeleteModalOpen = false;
            return;
        }
        
        try
        {
            var result = await TrainingFormatService.ActivateDeactivateTrainingFormat(DeleteTrainingFormatDto.Id);
            
            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }
            
            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await OnInitializedAsync();
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
                    IsDeleteModalOpen = false;
                    break;
                case StatusCode.Status401Unauthorized:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Warning, Variant.Outlined);
                    break;
                case StatusCode.Status400BadRequest:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Warning, Variant.Outlined);
                    break;
                case StatusCode.Status404NotFound:
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