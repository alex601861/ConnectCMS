using MudBlazor;
using Blazorise.Extensions;
using CMSTrain.Client.Models.Base;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Requests.Email;
using CMSTrain.Client.Models.Requests.TrainingCandidate;
using CMSTrain.Client.Models.Responses.ClientOrganization;
using CMSTrain.Client.Models.Responses.TrainingCandidate;

namespace CMSTrain.Client.Pages.Client.Training;

public partial class AvailableTrainings : ComponentBase
{
    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        
        await GetAllTrainingsForClient();
    }
    
    // TODO: Implementation of Component Based Through Out
    private bool IsDisplayedAsGrid { get; set; } = true;

    private void HandleDisplayFormatChange(bool value)
    {
        IsDisplayedAsGrid = value;
        
        StateHasChanged();
    }

    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.AvailableTrainings;
    }
    #endregion

    #region Get Client Users 
    private List<GetClientOrganizationUsersDto> ClientUsers { get; set; } = [];

    private async Task GetUsersForClientOrganization(Guid trainingId, int requestAction)
    {
        try
        {
            var result = await TrainingCandidateService.GetAllClientCandidatesForTraining(trainingId, requestAction);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            ClientUsers = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
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
        
        await GetAllTrainingsForClient();
    }
    #endregion
    
    #region Get All Trainings for Client Organizations
    private CollectionDto<GetAllTrainingsForClient>? TrainingDetails { get; set; }
    
    private int PageNumber { get; set; } = Constants.Pagination.Page;
    
    private int PageSize { get; set; } = Constants.Pagination.Size; 

    private async Task UpdatePageNumber(int pageNumber)
    {
        PageNumber = pageNumber;
        
        TrainingDetails = null;
        
        await GetAllTrainingsForClient();
    }
    
    private async Task UpdatePageSize(int pageSize)
    {
        PageNumber = 1;
        
        PageSize = pageSize;
        
        TrainingDetails = null;
        
        await GetAllTrainingsForClient();
    }
    
    private async Task GetAllTrainingsForClient()
    {
        try
        {
            var response = await TrainingService.GetAllTrainingsForClient(Constants.RequestAction.Available, PageNumber, PageSize, Search);

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            TrainingDetails = response;
            
            foreach (var training in TrainingDetails.Result)
            {
                training.ImageUrl = training.ImageUrl != null 
                    ? FileManager.FetchFileUrl(training.ImageUrl, Constants.FilePath.TrainingsImagesFilePath) 
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

    #region Navigate To Training Details
    private void NavigateToAssignedTrainingDetails(Guid trainingId)
    {
        NavigationManager.NavigateTo($"/client/assigned-trainings/training-details/{trainingId}");
    }
    
    private void NavigateToUnassignedTrainingDetails(Guid trainingId)
    {
        NavigationManager.NavigateTo($"client/available-trainings/training-details/{trainingId}");
    }
    #endregion

    #region Open Client Request Modal
    private bool ClientRequest { get; set; }

    private IReadOnlyCollection<Guid> SelectedClientNominations { get; set; } = [];

    private ClientCandidateAssignmentDto ClientCandidateAssignment { get; set; } = new();
    
    private async Task OpenClientRequestModal(Guid trainingId)
    {
        SelectedClientNominations = [];
        
        ClientCandidateAssignment = new ClientCandidateAssignmentDto
        {
            TrainingId = trainingId
        };

        SelectedClientNominations = [];
        
        await GetUsersForClientOrganization(ClientCandidateAssignment.TrainingId, Constants.RequestAction.Available);
        
        await GetClientOrganizationCandidateCount(ClientCandidateAssignment.TrainingId);
        
        OpenCloseClientRequestModal();
    }

    private void OpenCloseClientRequestModal()
    {
        ClientRequest = !ClientRequest;
        
        StateHasChanged();
    }
    
    private bool _isCandidateNotAssignedDisabled;

    private bool IsCandidateNotAssignedDisabled
    {
        get => _isCandidateNotAssignedDisabled || 
               SelectedClientNominations.IsNullOrEmpty();
        set => _isCandidateNotAssignedDisabled = value;
    }
    
    private void HandleAssignmentBusySubmitting(bool isBusySubmitting)
    {
        IsCandidateNotAssignedDisabled = isBusySubmitting;
        
        StateHasChanged();
    }
    #endregion

    #region Close Client Request Modal 
    private async Task CloseClientRequestModal(bool isClosed)
    {
        try
        {
            HandleAssignmentBusySubmitting(true);

            if (isClosed)
            {
                OpenCloseClientRequestModal();
                return;
            }

            ClientCandidateAssignment.CandidateIds = SelectedClientNominations.ToList();

            var result = await TrainingCandidateService.ClientCandidateAssignment(ClientCandidateAssignment);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await GetAllTrainingsForClient();
                    await CandidateAssignmentConfirmation(ClientCandidateAssignment.CandidateIds);
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
            HandleAssignmentBusySubmitting(false);
        }
    }
    
    private async Task CandidateAssignmentConfirmation(List<Guid> candidateIds)
    {
        try
        {
            HandleAssignmentBusySubmitting(true);

            var trainingAssignmentRequest = new TrainingRequestsActionRequestDto()
            {
                TrainingId = ClientCandidateAssignment.TrainingId,
                RequestActions = candidateIds.Select(x => new RequestAction()
                {
                    UserId = x,
                    Remarks = "",
                    IsApproved = true
                }).ToList()
            };

            var result = await EmailConfirmationService.TrainingRequestAction(trainingAssignmentRequest);

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
                case StatusCode.Status400BadRequest:
                case StatusCode.Status401Unauthorized:
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
            OpenCloseClientRequestModal();
            HandleAssignmentBusySubmitting(false);
        }
    }
    #endregion

    #region Get User Display Name
    private static string GetUserDisplayName(GetClientOrganizationUsersDto user)
    {
        return $"{user.Name} ({user.EmailAddress})";
    }
    #endregion
    
    #region Training Candidate Badge Count
    private GetClientOrganizationCandidateCountDto ClientOrganizationCount { get; set; } = new();
    
    private async Task GetClientOrganizationCandidateCount(Guid trainingId)
    {
        try
        {
            var response = await TrainingCandidateService.GetClientOrganizationCandidateCount(trainingId);

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            ClientOrganizationCount = response.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion
}