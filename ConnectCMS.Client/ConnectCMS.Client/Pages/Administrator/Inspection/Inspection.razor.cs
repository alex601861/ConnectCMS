using MudBlazor;
using CMSTrain.Client.Models.Base;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Service.Extensions;
using Microsoft.AspNetCore.Components.Forms;
using CMSTrain.Client.Models.Requests.Inspection;
using CMSTrain.Client.Models.Responses.Inspection;

namespace CMSTrain.Client.Pages.Administrator.Inspection;

public partial class Inspection : ComponentBase
{
    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        await GetAllInspection();
    }

    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.Inspection;
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
        
        await GetAllInspection();
    }
    
    private bool? IsActive { get; set; } = Constants.ActivationStatus.Active;

    private bool IsDisplayedAsGrid { get; set; } = true;

    private void HandleDisplayFormatChange(bool value)
    {
        IsDisplayedAsGrid = value;
        
        StateHasChanged();
    }
    
    private async Task OnStatusFilter(bool? isActive)
    {
        IsActive = isActive; 
        
        PageSize = Constants.Pagination.Size;

        PageNumber = Constants.Pagination.Page;
        
        Inspections = null;

        await GetAllInspection();
    }
    #endregion
    
    #region Inspection Details
    private CollectionDto<GetInspectionDto>? Inspections { get; set; }
    
    private int PageNumber { get; set; } = Constants.Pagination.Page;
    
    private int PageSize { get; set; } = Constants.Pagination.Size; 
    
    private async Task UpdatePageNumber(int pageNumber)
    {
        PageNumber = pageNumber;
        
        Inspections = null;
        
        await GetAllInspection();
    }
    
    private async Task UpdatePageSize(int pageSize)
    {
        PageNumber = 1;
        
        PageSize = pageSize;
        
        Inspections = null;
        
        await GetAllInspection();
    }
    
    private async Task GetAllInspection()
    {
        try
        {
            var response = await InspectionService.GetAllInspections(PageNumber, PageSize, Search, IsActive);

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            Inspections = response;
        
            if (Inspections is { Result: not null })
            {
                foreach (var inspection in Inspections.Result)
                {
                    inspection.ImageUrl = !string.IsNullOrEmpty(inspection.ImageUrl) 
                        ? FileManager.FetchFileUrl(inspection.ImageUrl, Constants.FilePath.InspectionImagesFilePath)
                        : "images/dummy-img.png";
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

    #region Create 
    private bool IsCreateModalOpen { get; set; }

    private CreateInspectionDto CreateInspectionDto { get; set; } = new();
    
    private bool _isCreateButtonDisabled;

    private bool IsCreateInspectionButtonDisabled
    {
        get => _isCreateButtonDisabled ||
               string.IsNullOrEmpty(CreateInspectionDto.Name) ||
               string.IsNullOrEmpty(CreateInspectionDto.Description) ||
               CreateInspectionDto.Image is null ||
               CreateInspectionDto.InspectionType == InspectionType.None;
        set => _isCreateButtonDisabled = value;
    }
    
    private void HandleInspectionBusySubmit(bool isBusySubmitting)
    {
        IsCreateInspectionButtonDisabled = isBusySubmitting;
        
        StateHasChanged();
    }

    private void OpenCloseCreateInspectionModal()
    {
        IsCreateModalOpen = !IsCreateModalOpen;

        StateHasChanged();
    }

    private void OpenCreateInspectionModal()
    {
        CreateInspectionDto = new CreateInspectionDto();

        OpenCloseCreateInspectionModal();
    }

    private async Task InsertInspection(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseCreateInspectionModal();

            return;
        }

        try
        {
            HandleInspectionBusySubmit(true);

            var result = await InspectionService.InsertInspection(CreateInspectionDto);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);

                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await GetAllInspection();
                    OpenCloseCreateInspectionModal();
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
            HandleInspectionBusySubmit(false);
        }
    }
    
    private void OnHandleInspectionUpload(IBrowserFile? file)
    {
        if (file is null) return;
        
        CreateInspectionDto.Image = file;
        
        UpdateInspectionDto.Image = file;
    }
    #endregion

    #region Details
    private GetInspectionDto GetInspection { get; set; } = new();

    private bool IsDetailsModalOpen { get; set; }

    private void OpenCloseInspectionDetailsModal()
    {
        IsDetailsModalOpen = !IsDetailsModalOpen;

        StateHasChanged();
    }

    // private async Task GetInspectionById(Guid inspectionId)
    // {
    //     var response = await InspectionService.GetInspectionById(inspectionId);
    //
    //     if (response?.Result is null)
    //     {
    //         SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
    //         return;
    //     }
    //
    //     GetInspection = response.Result;
    //
    //     OpenCloseInspectionDetailsModal();
    // }
    #endregion

    #region Update 
    private bool IsEditModalOpen { get; set; }

    private UpdateInspectionDto UpdateInspectionDto { get; set; } = new();
    
    private bool _isUpdateButtonDisabled;
    
    private bool IsUpdateInspectionButtonDisabled
    {
        get => _isUpdateButtonDisabled || 
               string.IsNullOrEmpty(UpdateInspectionDto.Name) ||
               string.IsNullOrEmpty(UpdateInspectionDto.Description) ||
               UpdateInspectionDto.Image is null ||
               UpdateInspectionDto.InspectionType == InspectionType.None;
        set => _isUpdateButtonDisabled = value;
    }
    
    private void HandleInspectionsUpdateBusySubmit(bool isBusySubmitting)
    {
        IsUpdateInspectionButtonDisabled = isBusySubmitting;
        
        StateHasChanged();
    }

    private void OpenCloseEditModal()
    {
        IsEditModalOpen = !IsEditModalOpen;

        StateHasChanged();
    }

    // private async Task OpenInspectionUpdateModal(Guid inspection)
    // {
    //     var response = await InspectionService.GetInspectionById(inspection);
    //
    //     if (response?.Result is null)
    //     {
    //         SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
    //         return;
    //     }
    //
    //     GetInspection = response.Result;
    //
    //     UpdateInspectionDto = new UpdateInspectionDto()
    //     {
    //         Id = GetInspection.Id,
    //         Name = GetInspection.Name,
    //         Description = GetInspection.Description,
    //     };
    //
    //     OpenCloseEditModal();
    // }

    private async Task UpdateInspection(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseEditModal();

            return;
        }

        try
        {
            HandleInspectionsUpdateBusySubmit(true);

            var result = await InspectionService.UpdateInspection(UpdateInspectionDto);

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
                    await GetAllInspection();
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
            HandleInspectionsUpdateBusySubmit(false);
        }
    }
    #endregion

    #region Delete
    private bool IsDeleteModalOpen { get; set; }

    private GetInspectionDto DeleteInspectionDto { get; set; } = new();

    private void OpenCloseDeleteModal()
    {
        IsDeleteModalOpen = !IsDeleteModalOpen;

        StateHasChanged();
    }

    // private async Task OpenInspectionDeleteModal(Guid inspection)
    // {
    //     var response = await InspectionService.GetInspectionById(inspection);
    //
    //     if (response?.Result is null)
    //     {
    //         SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
    //         return;
    //     }
    //
    //     DeleteInspectionDto = response.Result;
    //
    //     OpenCloseDeleteModal();
    // }

    private async Task DeleteInspection(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseDeleteModal();
            return;
        }

        try
        {
            var result = await InspectionService.ActivateDeactivateInspection(DeleteInspectionDto.Id);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    OpenCloseDeleteModal();
                    await GetAllInspection();
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

    #region Questionnaire Navigation
    private void NavigateToInspectionQuestionnaire(GetInspectionDto inspection)
    {
        var navigationUrl = inspection.Type.ToInspectionTypeString() switch
        {
            InspectionType.SwotAnalysis => "/strategic-traits",
            InspectionType.PersonalityTest => "/personality-test/questionnaire",
            InspectionType.Feedback => "/feedbacks/questionnaire",
            InspectionType.PersonalAssessment => "/personal-assessments/questionnaire",
            InspectionType.Others => "inspections/other/questionnaire",
            _ => throw new ArgumentException($"Unknown Inspection Type: {inspection.Type}")
        };
        
        NavigationManager.NavigateTo(navigationUrl);
    }
    #endregion
}