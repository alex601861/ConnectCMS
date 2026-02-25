using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.Answers;
using CMSTrain.Client.Models.Responses.TrainingInspection;

namespace CMSTrain.Client.Pages.Administrator.Questionnaire;

public partial class QuestionnaireStatistics
{
    [Parameter] public Guid QuestionnaireId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await GetTrainingAndInspectionDetails();
        
        await GetGeneralQuestionnaireAnswerResponseDto();
    }

    #region Search
    private string Search { get; set; } = string.Empty;
    #endregion
    
    #region Phase Statistics
    private int Phase { get; set; } = 1;

    private async Task OnStatisticsFilter()
    {
        await GetGeneralQuestionnaireAnswerResponseDto();
    }
    #endregion

    #region Training Inspection Details
    private GetTrainingInspectionDetailsDto TrainingInspection { get; set; } = new();

    private async Task GetTrainingAndInspectionDetails()
    {
        try
        {
            var trainingInspection = await TrainingInspectionService.GetTrainingInspectionByQuestionnaire(QuestionnaireId);

            if (trainingInspection?.Result is null)
            {
                SnackbarService.ShowSnackbar(trainingInspection?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                
                return;
            }

            TrainingInspection = trainingInspection.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion    
    
    #region General Questionnaire Responses with Statistics
    private GeneralQuestionnaireAnswerResponseDto GeneralQuestionnaireAnswer { get; set; } = new();
    
    private async Task GetGeneralQuestionnaireAnswerResponseDto()
    {
        try
        {
            var result = await QuestionnaireService.GetGeneralQuestionnaireAnswerResponses(QuestionnaireId, Phase);
            
            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Warning, Variant.Outlined);
                return;
            }
            
            GeneralQuestionnaireAnswer = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion
}