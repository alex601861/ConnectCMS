using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Extensions;
using CMSTrain.Client.Models.Responses.Answers;
using CMSTrain.Client.Models.Responses.Inspection;
using CMSTrain.Client.Models.Responses.TrainingInspection;

namespace CMSTrain.Client.Pages.Administrator.Questionnaire;

public partial class QuestionnaireResponses : ComponentBase
{
    [Parameter] public Guid QuestionnaireId { get; set; }
    
    [Parameter] public Guid UserResponseId { get; set; }

    [Parameter] public GetInspectionDto Inspection { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        await GetQuestionnaireRespondents();

        await GetTrainingAndInspectionDetails();
    }

    #region Search
    private string Search { get; set; } = string.Empty;
    #endregion
    
    #region Questionnaire Respondents
    private int Phase { get; set; } = 1;

    private async Task OnRespondentsFilter()
    {
        await GetQuestionnaireRespondents();
    }
    
    private List<GetResponseUserDetails> Respondents { get; set; } = new();
    
    private async Task GetQuestionnaireRespondents()
    {
        try
        {
            var result = await AnswerService.GetResponseUserDetails(QuestionnaireId, Phase);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Warning, Variant.Outlined);
                return;
            }

            Respondents = result.Result;
            
            StateHasChanged();
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
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

    private void NavigateToAnswerDetails(Guid userResponseId)
    {
        var strategyType = Inspection.Type.ToInspectionTypeString();
        
        NavigationManager.NavigateTo(strategyType switch
        {
            InspectionType.SwotAnalysis => $"strategic-trait-responses/{userResponseId}",
            InspectionType.PersonalityTest => $"personality-test-response/{userResponseId}",
            InspectionType.Feedback => $"feedback-details-form/{userResponseId}",
            InspectionType.PersonalAssessment => $"assessment-details-form/{userResponseId}",
            _ => $"answer-details-form/{userResponseId}"
        });
    }
}