using CMSTrain.Client.Layout.Application;
using MudBlazor;
using CMSTrain.Client.Models.Base;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Extensions;
using Microsoft.AspNetCore.Components.Forms;
using CMSTrain.Client.Models.Requests.Training;
using CMSTrain.Client.Models.Responses.Training;
using CMSTrain.Client.Models.Responses.TrainingFormat;
using Microsoft.JSInterop;

namespace CMSTrain.Client.Pages.Administrator.Training;

public partial class TrainingList
{
    [Parameter] public int StatusAction { get; set; } = Constants.StatusAction.All;

    [Parameter] public EventCallback<bool?> OnTrainingsCountUpdate { get; set; }
    
    // TODO: Implementation of Component Based Through Out
    private bool IsDisplayedAsGrid { get; set; } = true;

    private void HandleDisplayFormatChange(bool value)
    {
        IsDisplayedAsGrid = value;
        
        StateHasChanged();
    }

    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        
        await GetTrainings();
        
        await GetAllTrainingFormats();
    }
    
    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.Training;
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

        await GetTrainings();

        StateHasChanged();
    }

    private bool? IsActive { get; set; } = Constants.ActivationStatus.Active;

    private async Task OnStatusFilter(bool? isActive)
    {
        IsActive = isActive; 
        
        PageSize = Constants.Pagination.Size;

        PageNumber = Constants.Pagination.Page;
        
        Trainings = null;

        await GetTrainings();
    }
    #endregion

    #region Trainings
    private CollectionDto<GetTrainingDto>? Trainings { get; set; }
    
    private int PageNumber { get; set; } = Constants.Pagination.Page;
    
    private int PageSize { get; set; } = Constants.Pagination.Size; 

    private async Task UpdatePageNumber(int pageNumber)
    {
        PageNumber = pageNumber;
        
        Trainings = null;

        await GetTrainings(); 
    }

    private async Task UpdatePageSize(int pageSize)
    {
        PageNumber = 1;
        
        PageSize = pageSize;
        
        Trainings = null;
        
        await GetTrainings();
    }

    private async Task GetTrainings()
    {
        await OnTrainingsCountUpdate.InvokeAsync(IsActive);
        
        var response = await TrainingService.GetAllTrainings(StatusAction, PageNumber, PageSize, Search, IsActive);

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        Trainings = response;

        foreach (var training in Trainings.Result)
        {
            training.ImageUrl = training.ImageUrl != null
                ? FileManager.FetchFileUrl(training.ImageUrl, Constants.FilePath.TrainingsImagesFilePath)
                : "images/dummy-img.png";
        }
    }

    #endregion
    
    #region Training Status
    private async Task UpdateTrainingStatus(Guid trainingId)
    {
        try
        {
            var result = await TrainingService.ActivateDeactivateTraining(trainingId);
            
            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }
            
            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Warning, Variant.Outlined);
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
    
    #region Create
    private bool IsCreateModalOpen { get; set; }
    
    private CreateTrainingDto CreateTrainingDto { get; set; } = new();

    private void OpenCreateTrainingModal()
    {
        IsCreateModalOpen = true;
        CreateTrainingDto = new();
        StateHasChanged();
    }
    
    private bool _isCreateButtonDisabled;

    private bool IsCreateTrainingButtonDisabled
    {
        get => _isCreateButtonDisabled ||
               string.IsNullOrEmpty(CreateTrainingDto.Title) ||
               string.IsNullOrEmpty(CreateTrainingDto.Description) ||
               string.IsNullOrEmpty(CreateTrainingDto.LocationDetails) ||
               CreateTrainingDto.Longitude == null ||
               CreateTrainingDto.Latitude == null ||
               CreateTrainingDto.TrainingFormatId == Guid.Empty ||
               CreateTrainingDto.StartDate == null ||
               CreateTrainingDto.EndDate == null;
        set => _isCreateButtonDisabled = value;
    }
    
    private void HandleTrainingBusySubmit(bool isBusySubmitting)
    {
        IsCreateTrainingButtonDisabled = isBusySubmitting;
        
        StateHasChanged();
    }
    
    private async Task ToggleTrainingLocation(bool isForCreateAction)
    {
        var location = isForCreateAction ? CreateTrainingDto.LocationDetails : UpdateTrainingDto.LocationDetails;

        if (string.IsNullOrEmpty(location))
        {
            SnackbarService.ShowSnackbar("Please enter a valid location before redirecting to the respective location", Severity.Warning, Variant.Outlined);
            return;
        };
        
        var googleMapsUrl = $"https://www.google.com/maps/search/?api=1&query={Uri.EscapeDataString(location)}";

        await JsRuntime.InvokeVoidAsync("openInNewTab", googleMapsUrl);
    }
    
    private async Task NavigateToCoordinates(bool isForCreateAction)
    {
        var latitude = 0m;
        var longitude = 0m;
        
        if (isForCreateAction)
        {
            latitude = CreateTrainingDto.Latitude ?? 0m;
            longitude = CreateTrainingDto.Longitude ?? 0m;
        }
        else
        {
            latitude = UpdateTrainingDto.Latitude ?? 0m;
            longitude = UpdateTrainingDto.Longitude ?? 0m;        
        }
        
        var googleMapsUrl = $"https://www.google.com/maps?q={latitude},{longitude}";

        if (longitude == 0m || latitude == 0m)
        {
            SnackbarService.ShowSnackbar("Please enter a valid coordinates before redirecting to the respective location", Severity.Warning, Variant.Outlined);
            return;
        }
        
        await JsRuntime.InvokeVoidAsync("openInNewTab", googleMapsUrl);
        
    }
    
    private async Task InsertTraining(bool isClosed)
    {
        if (isClosed)
        {
            IsCreateModalOpen = false;
            return;
        }

        try
        {
            HandleTrainingBusySubmit(true);

            var result = await TrainingService.InsertTraining(CreateTrainingDto);

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
                    await OnTrainingsCountUpdate.InvokeAsync(IsActive);
                    IsCreateModalOpen = false;
                    await GetTrainings();
                    StateHasChanged();
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
            HandleTrainingBusySubmit(false);
        }
    }
    
    private void HandleCreateTrainingImageUpload(IBrowserFile? file)
    {
        CreateTrainingDto.Image = file;
    }

    private void OnCreateTrainingStartDateChange(DateTime? startDate)
    {
        if (startDate is null) return;

        CreateTrainingDto.StartDate = startDate.Value;
        CreateTrainingDto.EndDate = startDate.Value.AddDays(1);
        
        StateHasChanged();
    }
    #endregion

    #region Edit
    private GetTrainingDto TrainingDto { get; set; } = new();
    
    private bool IsEditModalOpen { get; set; }
    
    private List<GetTrainingFormatDto> TrainingFormats { get; set; } = new();

    private UpdateTrainingDto UpdateTrainingDto { get; set; } = new();
    
    private bool _isUpdateButtonDisabled;
    
    private bool IsUpdateTrainingButtonDisabled
    {
        get => _isUpdateButtonDisabled || 
               string.IsNullOrEmpty(UpdateTrainingDto.Title) ||
               string.IsNullOrEmpty(UpdateTrainingDto.Description) ||
               string.IsNullOrEmpty(UpdateTrainingDto.LocationDetails) ||
               CreateTrainingDto.Longitude == 0 ||
               CreateTrainingDto.Latitude == 0 ||
               UpdateTrainingDto.TrainingFormatId == Guid.Empty ||
               UpdateTrainingDto.StartDate == null ||
               UpdateTrainingDto.EndDate == null;
        set => _isUpdateButtonDisabled = value;
    }
    
    private void HandleTrainingUpdateBusySubmit(bool isBusySubmitting)
    {
        IsUpdateTrainingButtonDisabled = isBusySubmitting;
        
        StateHasChanged();
    }
    
    private async Task GetAllTrainingFormats()
    {
        var response = await TrainingFormatService.GetTrainingFormats(isActive: true);
        
        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        TrainingFormats = response.Result;
    }

    private async Task OpenTrainingUpdateModal(Guid trainingId)
    {
        var response = await TrainingService.GetTrainingById(trainingId);
        
        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }
        
        TrainingDto = response.Result;

        UpdateTrainingDto = new UpdateTrainingDto()
        {
            Id = TrainingDto.Id,
            Title = TrainingDto.Title,
            Description = TrainingDto.Description,
            LocationDetails = TrainingDto.LocationDetails,
            TrainingFormatId = TrainingDto.TrainingFormatId,
            Longitude = TrainingDto.Longitude,
            Latitude = TrainingDto.Latitude,
            StartDate = TrainingDto.StartDate.ToDateTimeFromDateOnlyString(),
            EndDate = TrainingDto.EndDate.ToDateTimeFromDateOnlyString(),
        };

        IsEditModalOpen = true;
        
        StateHasChanged();
    }
    
    private async Task UpdateTraining(bool isClosed)
    {
        if (isClosed)
        {
            IsEditModalOpen = false;
            return;
        }

        try
        {
            HandleTrainingUpdateBusySubmit(true);

            var result = await TrainingService.UpdateTraining(UpdateTrainingDto);

            switch (result?.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await OnInitializedAsync();
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
                    IsEditModalOpen = false;
                    break;
                case StatusCode.Status401Unauthorized:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
                    break;
                case StatusCode.Status400BadRequest:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Warning, Variant.Outlined);
                    break;
                case StatusCode.Status404NotFound:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
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
            HandleTrainingUpdateBusySubmit(false);
        }
    }
    
    private void HandleUpdateTrainingImageUpload(IBrowserFile? file)
    {
        UpdateTrainingDto.Image = file;
    }
    
    private void OnUpdateTrainingStartDateChange(DateTime? startDate)
    {
        if (startDate is null) return;

        UpdateTrainingDto.StartDate = startDate.Value;
        UpdateTrainingDto.EndDate = startDate.Value.AddDays(1);
        
        StateHasChanged();
    }
    #endregion

    #region Delete
    private bool IsDeleteModalOpen { get; set; }
    
    private GetTrainingDto DeleteTrainingDto { get; set; } = new();

    private async Task OpenTrainingDeleteModal(Guid trainingFormatId)
    {
        var response = await TrainingService.GetTrainingById(trainingFormatId);
        
        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        DeleteTrainingDto = response.Result;

        IsDeleteModalOpen = true;
        
        StateHasChanged();
    }
    
    private async Task DeleteTraining(bool isClosed)
    {
        if (isClosed)
        {
            IsDeleteModalOpen = false;
            return;
        }
        try
        {
            var result = await TrainingService.ActivateDeactivateTraining(DeleteTrainingDto.Id);
            
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
    }
    #endregion

    #region Details
    private void NavigateToDetails(Guid trainingId)
    {
        if (trainingId == Guid.Empty)
        {
            SnackbarService.ShowSnackbar(Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }
        
        NavigationManager.NavigateTo($"trainings/admin/training-details/{trainingId}");
    }
    #endregion
}