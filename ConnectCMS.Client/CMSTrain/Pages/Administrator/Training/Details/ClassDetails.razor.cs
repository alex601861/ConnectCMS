using CMSTrain.Client.Models.Base;
using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Requests.Class;
using CMSTrain.Client.Models.Responses.Class;
using CMSTrain.Client.Models.Responses.Training;
using CMSTrain.Client.Service.Extensions;
using Microsoft.AspNetCore.Components.Forms;

namespace CMSTrain.Client.Pages.Administrator.Training.Details;

public partial class ClassDetails : ComponentBase
{
    [Parameter] public Guid TrainingId { get; set; }
    
    [Parameter] public EventCallback OnClassDetailsCountUpdate { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await GetTrainingClassDetails();
        await GetTrainingDetails();
    }
    
    // TODO: Implementation of Component Based Through Out
    private bool IsDisplayedAsGrid { get; set; } = true;

    private void HandleDisplayFormatChange(bool value)
    {
        IsDisplayedAsGrid = value;
        
        StateHasChanged();
    }

    #region Training Details
    private GetTrainingDto Training { get; set; } = new();

    private DateTime StartDate { get; set; } = new();

    private DateTime EndDate { get; set; } = new();
    
    private async Task GetTrainingDetails()
    {
        try
        {
            var response = await TrainingService.GetTrainingById(TrainingId);

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            Training = response.Result;

            StartDate = Training.StartDate.ToDateTimeFromDateOnlyString();
            EndDate = Training.EndDate.ToDateTimeFromDateOnlyString();
            
            CreateClass.Date = StartDate;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
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

        await GetTrainingClassDetails();
    }

    private int? Status { get; set; }

    private async Task OnClassDetailsFilter()
    {
        Classes = null;
        
        await GetTrainingClassDetails();
    }
    #endregion
    
    #region Class Details
    private CollectionDto<GetClassDto>? Classes { get; set; }
    
    private int PageNumber { get; set; } = Constants.Pagination.Page;
    
    private int PageSize { get; set; } = Constants.Pagination.Size; 

    private async Task UpdatePageNumber(int pageNumber)
    {
        PageNumber = pageNumber;
        
        Classes = null;
        
        await GetTrainingClassDetails();
    }
    
    private async Task UpdatePageSize(int pageSize)
    {
        PageNumber = 1;
        
        PageSize = pageSize;
        
        Classes = null;
        
        await GetTrainingClassDetails();
    }
    
    private async Task GetTrainingClassDetails()
    {
        try
        {
            var response = await ClassService.GetAllClasses(TrainingId, PageNumber, PageSize, Search, Status);

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            Classes = response;
            
            foreach (var classes in Classes.Result)
            {
                classes.ImageUrl = classes.ImageUrl != null 
                    ? FileManager.FetchFileUrl(classes.ImageUrl, Constants.FilePath.ClassesImagesFilePath) 
                    : "images/dummy-img.png";
            }
            
            StateHasChanged();
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion

    #region Insert Class
    private bool IsCreateClassModalOpen { get; set; }
    
    private CreateClassDto CreateClass { get; set; } = new();
    
    private bool _isCreateButtonDisabled;

    private bool IsCreateClassButtonDisabled
    {
        get => _isCreateButtonDisabled ||
               string.IsNullOrEmpty(CreateClass.Title) ||
               CreateClass.Date == null ||
               CreateClass.StartTime == null ||
               CreateClass.EndTime == null;
        set => _isCreateButtonDisabled = value;
    }
    
    private void HandleClassBusySubmit(bool isBusySubmitting)
    {
        IsCreateClassButtonDisabled = isBusySubmitting;
        
        StateHasChanged();
    }

    private void OpenCreateClassModal()
    {
        OpenCloseCreateClassModal();
        
        CreateClass = new()
        {
            TrainingId = TrainingId
        };
    }

    private async Task InsertClass(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseCreateClassModal();
            return;
        }

        try
        {
            HandleClassBusySubmit(true);

            var result = await ClassService.InsertClass(CreateClass);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    OpenCloseCreateClassModal();
                    await GetTrainingClassDetails();
                    await OnClassDetailsCountUpdate.InvokeAsync();
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
            HandleClassBusySubmit(false);
        }
    }

    private void OpenCloseCreateClassModal()
    {
        IsCreateClassModalOpen = !IsCreateClassModalOpen;

        StateHasChanged();
    }

    #endregion

    #region Update Class
    private bool IsUpdateClassModalOpen { get; set; }
    
    private UpdateClassDto UpdateClass { get; set; } = new();
    
    private bool _isUpdateButtonDisabled;
    
    private bool IsUpdateClassButtonDisabled
    {
        get => _isUpdateButtonDisabled || 
               string.IsNullOrEmpty(UpdateClass.Title) ||
               UpdateClass.Date == null || 
               UpdateClass.StartTime == null ||
               UpdateClass.EndTime == null;
        set => _isUpdateButtonDisabled = value;
    }
    
    private void HandleClassUpdateBusySubmit(bool isBusySubmitting)
    {
        IsUpdateClassButtonDisabled = isBusySubmitting;
        
        StateHasChanged();
    }

    private async Task OpenUpdateClassModal(Guid classId)
    {
        OpenCloseUpdateClassModal();

        var result = await ClassService.GetClassById(classId);

        if (result?.Result is null)
        {
            SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                Variant.Outlined);
            return;
        }

        var @class = result.Result;

        UpdateClass = new UpdateClassDto
        {
            Id = @class.Id,
            Title = @class.Title,
            Date = @class.Date.ToDateTimeFromDateOnlyString(),
            TrainingId = @class.TrainingId,
            StartTime = @class.StartTime.ToTimeSpanFromTimeSpanString(),
            EndTime = @class.EndTime.ToTimeSpanFromTimeSpanString()
        };
    }

    private async Task EditClass(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseUpdateClassModal();
            return;
        }

        try
        {
            HandleClassUpdateBusySubmit(true);

            var result = await ClassService.UpdateClass(UpdateClass);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await GetTrainingClassDetails();
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
                    OpenCloseUpdateClassModal();
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
            HandleClassUpdateBusySubmit(false);
        }
    }

    private void OpenCloseUpdateClassModal()
    {
        IsUpdateClassModalOpen = !IsUpdateClassModalOpen;
        StateHasChanged();
    }

    #endregion

    #region Image Uploads
    private void HandleCreateTrainingClassImageUpload(IBrowserFile? file)
    {
        CreateClass.Image = file;
    }
    
    private void HandleUpdateTrainingClassImageUpload(IBrowserFile? file)
    {
        UpdateClass.Image = file;
    }
    #endregion

    #region Delete
    private bool IsDeleteClassModalOpen { get; set; }
    private GetClassDto DeleteClassDto { get; set; } = new();

    private async Task OpenTrainingFormatDeleteModal(Guid classId)
    {
        var response = await ClassService.GetClassById(classId);

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        DeleteClassDto = response.Result;

        OpenCloseDeleteClassModal();
    }

    private async Task DeleteClass(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseDeleteClassModal();
            return;
        }

        try
        {
            var result = await ClassService.DeleteClass(DeleteClassDto.Id);

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
                    await GetTrainingClassDetails();
                    OpenCloseDeleteClassModal();
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

    private void OpenCloseDeleteClassModal()
    {
        IsDeleteClassModalOpen = !IsDeleteClassModalOpen;
        StateHasChanged();
    }

    #endregion

    #region Details
    private void NavigateToClassDetails(Guid classId)
    {
        NavigationManager.NavigateTo($"/trainings/admin-class-details/{classId}");
    }
    #endregion
}