using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Responses.Analysis;
using CMSTrain.Client.Models.Responses.TrainingInspection;

namespace CMSTrain.Client.Pages.Candidate.Questionnaire;

public partial class AssessmentEvaluationDetails
{
    [Parameter] public Guid QuestionnaireId { get; set; }
    
    [Parameter] public Guid UserResponseId { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();

        await GetTrainingInspectionDetails();
        
        await GetAssessmentEvaluationDetails();

        await GetAssessmentPhaseCount();
    }
    
    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.AssessmentEvaluationDetails;
    }
    #endregion

    #region Assessment Evaluation Details
    private List<GetAssessmentResponseAnalysisDto> AssessmentResponseAnalysis { get; set; } = new();
    
    private async Task GetAssessmentEvaluationDetails()
    {
        try
        {
            AssessmentResponseAnalysis = [];
            
            var result = await AnalysisService.GetUserResponseAnalysisEvaluationDetailsForAssessments(QuestionnaireId, UserResponseId,IsSubordinateRequired, Phase);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Warning, Variant.Outlined);
                return;
            }

            AssessmentResponseAnalysis = result.Result;
            
            StateHasChanged();
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    
    private int Phase { get; set; } = 1;
    
    private async Task OnPhaseChange(int phase)
    {
        Phase = phase;
        
        await GetAssessmentEvaluationDetails();
    }
    
    private bool IsSubordinateRequired { get; set; }

    private async Task OnSubordinateEvaluationChange(bool isSubordinateRequired)
    {
        IsSubordinateRequired = isSubordinateRequired;
        
        await GetAssessmentEvaluationDetails();
    }
    #endregion

    #region Assessment Phase Count
    private int PhaseCounts { get; set; } = 1;

    private async Task GetAssessmentPhaseCount()
    {
        try
        {
            var result = await TrainingInspectionService.GetTrainingInspectionPhaseCounts(TrainingInspectionDetails.TrainingInspectionId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Warning, Variant.Outlined);
                return;
            }

            PhaseCounts = result.Result ?? 1;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion

    #region Training Inspection Details
    private GetTrainingInspectionDetailsDto TrainingInspectionDetails { get; set; } = new();

    private async Task GetTrainingInspectionDetails()
    {
        try
        {
            var result = await TrainingInspectionService.GetTrainingInspectionByQuestionnaire(QuestionnaireId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Warning, Variant.Outlined);
                return;
            }

            TrainingInspectionDetails = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion
}