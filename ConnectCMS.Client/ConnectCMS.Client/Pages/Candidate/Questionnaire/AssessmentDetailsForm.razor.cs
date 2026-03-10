using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Responses.Answers;
using CMSTrain.Client.Models.Responses.Training;
using MudBlazor;

namespace CMSTrain.Client.Pages.Candidate.Questionnaire;

public partial class AssessmentDetailsForm
{
    [Parameter] public Guid UserResponseId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();

        await GetUserResponseDetails();

        await GetTrainingDetails();
    }

    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.AnswerDetailsForm;
    }
    #endregion

    #region Assessment Details
    private GetUserResponseDto UserResponse { get; set; } = new();

    private async Task GetUserResponseDetails()
    {
        try
        {
            var result = await AnswerService.GetUserResponseDetails(UserResponseId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Warning, Variant.Outlined);
                return;
            }

            UserResponse = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion

    #region Analysis Comparision Navigation
    private void NavigateToAnalysisComparision()
    {
        NavigationManager.NavigateTo($"/assessment-evaluation-form/{UserResponse.QuestionnaireId}/{UserResponse.UserResponseId}");
    }
    #endregion

    #region Training Details
    private GetTrainingDto TrainingDetails { get; set; } = new();

    private async Task GetTrainingDetails()
    {
        try
        {
            var result = await TrainingService.GetTrainingDetailsByQuestionnaire(UserResponse.QuestionnaireId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Warning, Variant.Outlined);
                return;
            }

            TrainingDetails = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion
}