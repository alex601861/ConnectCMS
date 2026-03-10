using MudBlazor;
using CMSTrain.Client.Models.Base;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Requests.Subordinate;
using CMSTrain.Client.Models.Responses.Subordinate;
using CMSTrain.Client.Models.Responses.TrainingCandidate;
using CMSTrain.Client.Models.Responses.TrainingInspection;

namespace CMSTrain.Client.Pages.Candidate.Training.Details;

public partial class SubordinateDetails : ComponentBase
{
    [Parameter] public Guid TrainingId {  get; set; }

    [Parameter] public TrainingCandidateAssignmentDetailsDto TrainingCandidateAssignment { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        await GetAllSubordinates();
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

        await GetAllSubordinates();
    }

    private int? Type { get; set; }

    private async Task OnSubordinatesFilter()
    {
        PageSize = Constants.Pagination.Size;

        PageNumber = Constants.Pagination.Page;

        Subordinates = null;
        
        await GetAllSubordinates();
    }
    #endregion
    
    #region Subordinate Details
    private CollectionDto<GetSubordinateDto>? Subordinates { get; set; }
    
    private int PageNumber { get; set; } = Constants.Pagination.Page;
    
    private int PageSize { get; set; } = Constants.Pagination.Size; 
    
    private async Task UpdatePageNumber(int pageNumber)
    {
        PageNumber = pageNumber;

        Subordinates = null;
        
        await GetAllSubordinates();
    }

    private async Task UpdatePageSize(int pageSize)
    {
        PageNumber = 1;
        
        PageSize = pageSize;

        Subordinates = null;
        
        await GetAllSubordinates();
    }
    
    private async Task GetAllSubordinates()
    {
        try
        {
            var response = await SubordinateService.GetSubordinateDetails(TrainingId, PageNumber, PageSize, Search, Type);

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            Subordinates = response;
            
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

    private CreateCandidateSubordinateDto CreateCandidateSubordinateDto { get; set; } = new();
    
    private bool _isCreateButtonDisabled;

    private bool IsCreateSubordinateButtonDisabled
    {
        get => _isCreateButtonDisabled || 
               string.IsNullOrEmpty(CreateCandidateSubordinateDto.SubordinateDetails.Name) ||
               string.IsNullOrEmpty(CreateCandidateSubordinateDto.SubordinateDetails.Email) ||
               string.IsNullOrEmpty(CreateCandidateSubordinateDto.SubordinateDetails.ContactNumber);
        set => _isCreateButtonDisabled = value;
    }
    
    private void HandleSubordinateCreateBusySubmit(bool isBusySubmitting)
    {
        IsCreateSubordinateButtonDisabled = isBusySubmitting;
        
        StateHasChanged();
    }

    private void OpenCloseCreateSubordinateModal()
    {
        IsCreateModalOpen = !IsCreateModalOpen;

        StateHasChanged();
    }

    private void OpenCreateSubordinateModal()
    {
        CreateCandidateSubordinateDto = new CreateCandidateSubordinateDto()
        {
            TrainingId = TrainingId
        };

        OpenCloseCreateSubordinateModal();
    }

    private async Task InsertSubordinate(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseCreateSubordinateModal();

            return;
        }

        try
        {
            HandleSubordinateCreateBusySubmit(true);

            var result = await SubordinateService.InsertSubordinateForCandidates(CreateCandidateSubordinateDto);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);

                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await GetAllSubordinates();
                    OpenCloseCreateSubordinateModal();
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
            HandleSubordinateCreateBusySubmit(false);
        }
    }
    #endregion

    #region Inspections
    private bool IsTrainingInspectionQuestionnaireDetailsModalOpen { get; set; }
    
    private GetSubordinateTrainingInspectionDto SubordinateTrainingInspectionDetails { get; set; } = new();
    
    private void OpenCloseTrainingInspectionQuestionnaireDetailsModal()
    {
        IsTrainingInspectionQuestionnaireDetailsModalOpen = !IsTrainingInspectionQuestionnaireDetailsModalOpen;
        
        StateHasChanged();
    }
    
    private async Task OpenTrainingInspectionQuestionnaireDetailsModal(Guid subordinateId)
    {
        try
        {
            var result = await TrainingInspectionService.GetSubordinateTrainingInspectionDetails(subordinateId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                
                return;
            }

            SubordinateTrainingInspectionDetails = result.Result ?? new GetSubordinateTrainingInspectionDto();
            
            SubordinateTrainingInspectionDetails.ImageUrl = SubordinateTrainingInspectionDetails.ImageUrl != null 
                ? FileManager.FetchFileUrl(SubordinateTrainingInspectionDetails.ImageUrl, Constants.FilePath.InspectionImagesFilePath) 
                : "images/dummy-img.png";
            
            OpenCloseTrainingInspectionQuestionnaireDetailsModal();
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion

    #region Navigation to Questionnaire and Answers
    private void CopyQuestionnaireAnswerUploadForm(Guid questionnaireId, Guid subordinateId)
    {
        var baseUri = NavigationManager.BaseUri;

        var fullUrl = $"{baseUri}subordinate-answer-upload-form/{questionnaireId}/{subordinateId}";
    
        ClipboardService.CopyTextToClipboard(fullUrl);
        
        SnackbarService.ShowSnackbar("The questionnaire link has been copied to the clipboard.", Severity.Success, Variant.Outlined);
    }
    
    private void NavigateToAnswerDetailsForm(Guid userResponseId)
    {
        NavigationManager.NavigateTo($"answer-details-form/{userResponseId}");
    }
    #endregion
}