using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.Answers;
using CMSTrain.Client.Models.Responses.Training;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CMSTrain.Client.Pages.Candidate.Questionnaire;

public partial class AnswerDetailsForm
{
    [Parameter] public Guid UserResponseId { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        
        await GetUserResponseDetails();

        await GetTrainingDetails();

        await GetNavigationDetails();
    }
    
    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.AnswerDetailsForm;
    }
    #endregion
    
    #region Navigation URL
    private string TrainingQuestionnaireUrl { get; set; } = string.Empty;

    private async Task GetNavigationDetails()
    {
        try
        {
            var response = await ProfileService.GetUserRole();

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            var role = response.Result;

            TrainingQuestionnaireUrl = role.Name switch
            {
                Constants.Roles.Client =>
                    $"/client/available-trainings",
                Constants.Roles.Trainer =>
                    $"/trainer/available-trainings",
                Constants.Roles.Candidate =>
                    $"/candidate/assigned-trainings/training-details/{TrainingDetails.Id}/5",
                Constants.Roles.SuperAdmin=>
                    $"/admin/questionnaire-view-form/{UserResponse.QuestionnaireId}",
                _ => TrainingQuestionnaireUrl
            };
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
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