using MudBlazor;
using CMSTrain.Client.Models.Base;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Requests.ClassTrainers;
using CMSTrain.Client.Models.Responses.ClassTrainers;

namespace CMSTrain.Client.Pages.State.Trainer;

public partial class TrainerDetails : ComponentBase
{
    [Parameter] public Guid ModuleId { get; set; }
    
    [Parameter] public bool IsTrainerForTraining { get; set; }

    [Parameter] public bool IsAssignable { get; set; }
    
    [Parameter] public EventCallback OnTrainerCountUpdate { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await GetAssignedTrainerDetails();
    }
    
    // TODO: Implementation of Component Based Through Out
    private bool IsDisplayedAsGrid { get; set; } = true;

    private void HandleDisplayFormatChange(bool value)
    {
        IsDisplayedAsGrid = value;
        
        StateHasChanged();
    }
    
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
        
        await GetAssignedTrainerDetails();
        
        StateHasChanged();
    }
    #endregion
    
    #region Trainer Details
    private CollectionDto<GetAssignedTrainersDto>? Trainers { get; set; }
    
    private int PageNumber { get; set; } = Constants.Pagination.Page;
    
    private int PageSize { get; set; } = Constants.Pagination.Size; 
    
    private async Task UpdatePageNumber(int pageNumber)
    {
        PageNumber = pageNumber;
        
        Trainers = null;
        
        await GetAssignedTrainerDetails();
    }

    private async Task UpdatePageSize(int pageSize)
    {
        PageNumber = 1;
        
        PageSize = pageSize;
        
        Trainers = null;
        
        await GetAssignedTrainerDetails();
    }

    private async Task GetAssignedTrainerDetails()
    {
        try
        {
            var response = IsTrainerForTraining 
                ? await ClassTrainerService.GetAllTrainersForTraining(ModuleId, PageNumber, PageSize, Search)
                : await ClassTrainerService.GetAllTrainersForClass(ModuleId, PageNumber, PageSize, Search);

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            Trainers = response;
            
            StateHasChanged();
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion
    
    #region Class Trainers
    private bool IsAssignTrainersModalOpen { get; set; }

    private bool _isAssignTrainersButtonDisabled;
    
    private bool IsAssignTrainersButtonDisabled
    {
        get => _isAssignTrainersButtonDisabled || AssignedTrainerIds.Count == 0;
        set => _isAssignTrainersButtonDisabled = value;
    }
    
    private void HandleAssignTrainersBusySubmit(bool isBusySubmitting)
    {
        IsAssignTrainersButtonDisabled = isBusySubmitting;
        
        StateHasChanged();
    }
    
    private IReadOnlyCollection<Guid> AssignedTrainerIds { get; set; } = [];
    
    private AssignTrainersDto AssignTrainers { get; set; } = new();

    private List<GetTrainersDto> TrainerList { get; set; } = [];

    private async Task GetAllTrainers()
    {
        try
        {
            var response = await TrainerService.GetAllActiveTrainers();

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            TrainerList = response.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    
    private async Task OpenClassAssignedTrainerDetails()
    {
        try
        {
            var trainerIds = Trainers?.Result.Select(x => x.Id).ToList()!;

            AssignTrainers = new AssignTrainersDto()
            {
                ClassId = ModuleId,
                TrainerIds = trainerIds
            };

            AssignedTrainerIds = trainerIds;
            
            OpenCloseAssignTrainersModal();

            await GetAllTrainers();
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }

    private async Task AssignTrainerToClasses(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseAssignTrainersModal();
            return;
        }

        try
        {
            HandleAssignTrainersBusySubmit(true);

            AssignTrainers.TrainerIds = AssignedTrainerIds.ToList();

            var result = await ClassTrainerService.AssignTrainersToClass(AssignTrainers);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    OpenCloseAssignTrainersModal();
                    await GetAssignedTrainerDetails();
                    await OnTrainerCountUpdate.InvokeAsync();
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
            HandleAssignTrainersBusySubmit(false);
        }
    }
    
    private void OpenCloseAssignTrainersModal()
    {
        IsAssignTrainersModalOpen = !IsAssignTrainersModalOpen;
        
        StateHasChanged();
    }
    
    private static string GetUserDisplayName(GetTrainersDto user)
    {
        return $"{user.Name} ({user.EmailAddress})";
    }
    #endregion
    
    #region Update Trainer Description
    private bool IsUpdateTrainerDescriptionModalOpen { get; set; }

    private bool _isUpdateDescriptionButtonDisabled;
    
    private bool IsUpdateDescriptionButtonDisabled
    {
        get => _isUpdateDescriptionButtonDisabled ||
               string.IsNullOrEmpty(UpdateClassTrainerDescription.Description);
        set => _isUpdateDescriptionButtonDisabled = value;
    }
    
    private void HandleUpdateDescriptionBusySubmit(bool isBusySubmitting)
    {
        IsUpdateDescriptionButtonDisabled = isBusySubmitting;
        
        StateHasChanged();
    }
    
    private UpdateClassTrainerDescriptionDto UpdateClassTrainerDescription { get; set; } = new();
    
    private void OpenCloseUpdateTrainingDescriptionModal(GetAssignedTrainersDto trainer)
    {
        IsUpdateTrainerDescriptionModalOpen = !IsUpdateTrainerDescriptionModalOpen;

        UpdateClassTrainerDescription = IsUpdateTrainerDescriptionModalOpen ? new()
        {
            ClassTrainerId  = trainer.ClassTrainerId,
            Description = trainer.Description ?? ""
        } : new UpdateClassTrainerDescriptionDto(); 
        
        HandleUpdateDescriptionBusySubmit(false);
        
        StateHasChanged();
    }
    
    private async Task UpdateCandidateFromTraining(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseUpdateTrainingDescriptionModal(new GetAssignedTrainersDto());

            return;
        }

        try
        {
            HandleUpdateDescriptionBusySubmit(true);

            var response = await ClassTrainerService.UpdateTrainerDescription(UpdateClassTrainerDescription);

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);
                return;
            }

            switch (response.StatusCode)
            {
                case StatusCode.Status200Ok:
                    SnackbarService.ShowSnackbar(response.Message, Severity.Success, Variant.Outlined);
                    OpenCloseUpdateTrainingDescriptionModal(new GetAssignedTrainersDto());
                    await GetAssignedTrainerDetails();
                    break;
                case StatusCode.Status401Unauthorized:
                case StatusCode.Status400BadRequest:
                case StatusCode.Status404NotFound:
                    SnackbarService.ShowSnackbar(response.Message, Severity.Warning, Variant.Outlined);
                    break;
                case StatusCode.Status500InternalServerError:
                    SnackbarService.ShowSnackbar(response.Message, Severity.Error, Variant.Outlined);
                    break;
            }
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
        finally
        {
            HandleUpdateDescriptionBusySubmit(false);
        }
    }
    #endregion

    #region View Trainer Description
    private bool IsTrainerDescriptionModalOpen { get; set; }
    
    private GetTrainerDescriptionDto TrainerDescriptions { get; set; } = new();

    private async Task OpenCloseTrainingDescriptionModal(Guid trainerId)
    {
        IsTrainerDescriptionModalOpen = !IsTrainerDescriptionModalOpen;

        if (IsTrainerDescriptionModalOpen)
        {
            var response = IsTrainerForTraining 
                ? await ClassTrainerService.GetTrainerDescriptionsOnTraining(ModuleId, trainerId) 
                : await ClassTrainerService.GetTrainerDescriptionsOnClass(ModuleId, trainerId);;
            
            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }
            
            TrainerDescriptions = response.Result;
            
            foreach (var classes in TrainerDescriptions.Classes)
            {
                classes.ImageUrl = classes.ImageUrl != null
                    ? FileManager.FetchFileUrl(classes.ImageUrl, Constants.FilePath.ClassesImagesFilePath)
                    : "images/dummy-img.png";
            }
        }
        else
        {
            TrainerDescriptions = new GetTrainerDescriptionDto();
        }
        
        StateHasChanged();
    }
    
    private async Task CloseTrainerDescriptionModal()
    {
        await OpenCloseTrainingDescriptionModal(Guid.Empty);
        
        StateHasChanged();
    }
    #endregion
}