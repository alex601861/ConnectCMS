using MudBlazor;
using CMSTrain.Client.Models.Base;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Extensions;
using CMSTrain.Client.Models.Responses.TrainingInspection;

namespace CMSTrain.Client.Pages.State.Candidate;

public partial class CandidateInspectionDetails
{
    [Parameter] public Guid TrainingCandidateId { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        await GetAllAssignedTrainingInspectionsForCandidate();
    }

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
        
        await GetAllAssignedTrainingInspectionsForCandidate();
    }
    #endregion
    
    #region Training Candidate Questionnaires (with Subordinates)
    private CollectionDto<GetTrainingInspectionDto>? Inspections { get; set; }
    
    private int PageNumber { get; set; } = Constants.Pagination.Page;
    
    private int PageSize { get; set; } = Constants.Pagination.Size; 

    private async Task UpdatePageNumber(int pageNumber)
    {
        PageNumber = pageNumber;
        
        Inspections = null;
        
        await GetAllAssignedTrainingInspectionsForCandidate();
    }

    private async Task UpdatePageSize(int pageSize)
    {
        PageNumber = 1;
        
        PageSize = pageSize;
        
        Inspections = null;
        
        await GetAllAssignedTrainingInspectionsForCandidate();
    }
    
    private async Task GetAllAssignedTrainingInspectionsForCandidate()
    {
        try
        {
            var result = await TrainingInspectionService.GetAllAssignedTrainingInspectionsForTrainingCandidate(TrainingCandidateId, PageNumber, PageSize, Search);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }
            
            Inspections = result;
            
            StateHasChanged();
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion

    #region Training Inspection Questionnaire Details
    private bool IsTrainingInspectionQuestionnaireDetailsModalOpen { get; set; }
    
    private GetCandidateTrainingInspectionDto CandidateTrainingInspectionDetails { get; set; } = new();
    
    private void OpenCloseTrainingInspectionQuestionnaireDetailsModal()
    {
        IsTrainingInspectionQuestionnaireDetailsModalOpen = !IsTrainingInspectionQuestionnaireDetailsModalOpen;
        
        StateHasChanged();
    }
    
    private async Task OpenTrainingInspectionQuestionnaireDetailsModal(Guid trainingInspectionId)
    {
        try
        {
            var result = await TrainingInspectionService.GetCandidateTrainingInspectionDetailsForTrainingCandidate(TrainingCandidateId, trainingInspectionId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                
                return;
            }

            CandidateTrainingInspectionDetails = result.Result ?? new GetCandidateTrainingInspectionDto();
            
            CandidateTrainingInspectionDetails.ImageUrl = CandidateTrainingInspectionDetails.ImageUrl != null 
                ? FileManager.FetchFileUrl(CandidateTrainingInspectionDetails.ImageUrl, Constants.FilePath.InspectionImagesFilePath) 
                : "images/dummy-img.png";
            
            OpenCloseTrainingInspectionQuestionnaireDetailsModal();
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion
    
    #region Questionnaire and Answer Navigation
    private void NavigateToAnswerDetailsForm(Guid userResponseId, string strategicType)
    {
        var strategyType = strategicType.ToInspectionTypeString();
        
        NavigationManager.NavigateTo(strategyType switch
        {
            InspectionType.SwotAnalysis => $"/strategic-trait-responses/{userResponseId}",
            InspectionType.PersonalityTest => $"/personality-test-response/{userResponseId}",
            InspectionType.PersonalAssessment => $"/assessment-details-form/{userResponseId}",
            InspectionType.Feedback => $"/feedback-details-form/{userResponseId}",
            _ => $"answer-details-form/{userResponseId}"
        });
    }
    #endregion
}