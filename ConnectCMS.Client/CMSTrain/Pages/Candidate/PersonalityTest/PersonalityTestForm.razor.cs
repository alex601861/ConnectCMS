using CMSTrain.Client.Layout.Application;
using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Requests.PersonalityTest;
using CMSTrain.Client.Models.Responses.PersonalityTest;
using CMSTrain.Client.Models.Responses.Questionnaires;
using CMSTrain.Client.Models.Responses.Training;

namespace CMSTrain.Client.Pages.Candidate.PersonalityTest;

public partial class PersonalityTestForm
{
    [Parameter] public Guid QuestionnaireId { get; set; }

    private int ActivePanelIndex { get; set; }
    
    private bool IsLoading { get; set; } = true;
    
    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        
        await GetQuestionnaireValidity();

        if (QuestionnaireValidity is { IsValid: true, IsAnswered: false })
        {
            await GetPersonalityTestQuestionnaires();
        }
        
        IsLoading = false;
    }
    
    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.PersonalityTestForm;
    }
    #endregion

    #region Questionnaire Validity
    private GetQuestionnaireValidityDto QuestionnaireValidity { get; set; } = new();
    
    private async Task GetQuestionnaireValidity()
    {
        try
        {
            var result = await QuestionnaireService.GetQuestionnaireValidity(QuestionnaireId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            QuestionnaireValidity = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion
    
    #region Questionnaires
    private GetPersonalityTestQuestionnaireDto QuestionnaireDetails { get; set; } = new();
    
    private async Task GetPersonalityTestQuestionnaires()
    {
        try
        {
            var result = await PersonalityTestService.GetPersonalityTestQuestionnaires(QuestionnaireId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            QuestionnaireDetails = result.Result;

            InitializeAnswers();
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion

    #region Tab Navigation and Validations
    private bool ValidateCurrentTab()
    {
        if (ActivePanelIndex < 0) return false;

        if (QuestionnaireDetails.QuestionnaireTraits.Count <= 0) return false;
        
        var currentTraitQuestions = QuestionnaireDetails.QuestionnaireTraits[ActivePanelIndex].Facets
            .SelectMany(f => f.Questions)
            .ToList();
            
        return currentTraitQuestions.All(q => 
            PersonalityTestQuestionnaireAnswers.Any(a => a.QuestionId == q.QuestionId && a.AnswerId != Guid.Empty));
    }

    private async Task NavigateTabs(bool isNext)
    {
        if (isNext)
        {
            if (ValidateCurrentTab())
            {
                ActivePanelIndex++;
            }
            else
            {
                SnackbarService.ShowSnackbar("Please select answers for all the questions before navigating to the next tab.", Severity.Warning, Variant.Outlined);
            }
        }
        else
        {
            ActivePanelIndex--;
        }

        await ScrollManager.ScrollToAsync(null, 0, 0, ScrollBehavior.Smooth);
    }
    #endregion
    
    #region Answers and Handling Approach
    private List<PersonalityTestQuestionnaire> PersonalityTestQuestionnaireAnswers { get; set; } = [];
    
    private void InitializeAnswers()
    {
        PersonalityTestQuestionnaireAnswers = QuestionnaireDetails.QuestionnaireTraits
            .SelectMany(t => t.Facets)
            .SelectMany(x => x.Questions)
            .Select(q => new PersonalityTestQuestionnaire
            {
                QuestionId = q.QuestionId,
                AnswerId = Guid.Empty
            })
            .ToList();
    }
    #endregion

    #region Personality Test Submission
    private bool IsDisabled { get; set; }
    
    private async Task OnHandlePersonalityTestSubmission()
    {
        try
        {
            IsDisabled = true;

            var personalityTest = new PersonalityTestRequestDto
            {
                QuestionnaireId = QuestionnaireId,
                Answers = PersonalityTestQuestionnaireAnswers
            };

            var result = await PersonalityTestService.UploadPersonalityTestAnswers(personalityTest);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);

                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    var training = await GetTrainingDetailsByQuestionnaire();
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
                    NavigationManager.NavigateTo($"/candidate/assigned-trainings/training-details/{training.Id}");
                    break;
                case StatusCode.Status401Unauthorized:
                case StatusCode.Status400BadRequest:
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
            IsDisabled = false;
        }
    }
    #endregion
    
    #region Training Details Navigation
    private async Task<GetTrainingDto> GetTrainingDetailsByQuestionnaire()
    {
        var result = await TrainingService.GetTrainingDetailsByQuestionnaire(QuestionnaireId);

        if (result?.Result is null)
        {
            SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);

            return new GetTrainingDto();
        }

        var training = result.Result;
        
        return training;
    }
    #endregion
}