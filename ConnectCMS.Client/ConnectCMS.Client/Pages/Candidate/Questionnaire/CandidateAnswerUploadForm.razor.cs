using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Requests.Answers;
using CMSTrain.Client.Models.Responses.Identity;
using CMSTrain.Client.Models.Responses.Training;
using CMSTrain.Client.Models.Responses.Inspection;
using CMSTrain.Client.Models.Requests.Questionnaires;
using CMSTrain.Client.Models.Responses.Questionnaires;
using CMSTrain.Client.Models.Responses.TrainingInspection;

namespace CMSTrain.Client.Pages.Candidate.Questionnaire;

public partial class CandidateAnswerUploadForm : ComponentBase
{
    [Parameter] public Guid QuestionnaireId { get; set; }
    
    private bool IsLoading { get; set; } = true;

    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();

        await GetQuestionnaireValidity();

        if (QuestionnaireValidity is { IsValid: true, IsAnswered: false })
        {
            var questionnaire = await GetQuestionnaireDetails();

            var trainingInspection = await GetTrainingAndInspectionDetails(questionnaire.TrainingInspectionId);

            await GetTrainingDetails(trainingInspection.TrainingId);

            await GetInspectionDetails(trainingInspection.InspectionId);
            
            await GetCandidateDetails();

            QuestionDetails = MapQuestionnaireDetailsToQuestionAnswers(questionnaire);
        }

        GetNavigationDetails();

        IsLoading = false;
    }

    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.AnswerUploadForm;
    }
    #endregion

    #region Navigation URL
    private string TrainingQuestionnaireUrl { get; set; } = string.Empty;

    private void GetNavigationDetails()
    {
        try
        {
            TrainingQuestionnaireUrl = $"/candidate/assigned-trainings/training-details/{Training.Id}/5";
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
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
    
    #region Questionnaire Details
    private GetCandidateQuestionnaireDto Questionnaire { get; set; } = new();

    private List<QuestionAnswerDetailsDto> QuestionDetails { get; set; } = [];

    private async Task<GetCandidateQuestionnaireDto> GetQuestionnaireDetails()
    {
        try
        {
            var questionnaires = await QuestionnaireService.GetAllQuestionnairesForCandidates(QuestionnaireId);

            if (questionnaires?.Result is null)
            {
                SnackbarService.ShowSnackbar(questionnaires?.Message ?? Constants.Message.ExceptionMessage, Severity.Warning, Variant.Outlined);

                return new GetCandidateQuestionnaireDto();
            }

            Questionnaire = questionnaires.Result;

            return Questionnaire;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }

        return new GetCandidateQuestionnaireDto();
    }
    
    private List<QuestionAnswerDetailsDto> MapQuestionnaireDetailsToQuestionAnswers(GetCandidateQuestionnaireDto questionnaireAnswers)
    {
        var result = new List<QuestionAnswerDetailsDto>();

        foreach (var questionnaire in questionnaireAnswers.Questions)
        {
            result.Add(new QuestionAnswerDetailsDto()
            {
                Heading = string.Empty,
                QuestionId = questionnaire.QuestionId,
                Title = questionnaire.Title,
                Type = questionnaire.Type,
                Answer = string.Empty,
                MultiSelectAnswerIds = [],
                Rating = 0,
                Answers = questionnaire.Answers.Select(x => new McqAnswerDetails()
                {
                    Id = x.Id, 
                    Title = x.Title
                }).ToList()
            });
        }
        
        foreach (var questionnaire in questionnaireAnswers.HeadingQuestions)
        {
            foreach (var headingQuestionnaire in questionnaire.Questions)
            {
                result.Add(new QuestionAnswerDetailsDto()
                {
                    Heading = questionnaire.Heading,
                    QuestionId = headingQuestionnaire.QuestionId,
                    Title = headingQuestionnaire.Title,
                    Type = headingQuestionnaire.Type,
                    Answer = string.Empty,
                    MultiSelectAnswerIds = [],
                    Rating = 0,
                    Answers = headingQuestionnaire.Answers.Select(x => new McqAnswerDetails()
                    {
                        Id = x.Id, 
                        Title = x.Title
                    }).ToList()
                });
            }
            
            foreach (var subheadingQuestionnaire in questionnaire.SubHeadingQuestions)
            {
                foreach (var subheadingQuestions in subheadingQuestionnaire.Questions)
                {
                    result.Add(new QuestionAnswerDetailsDto()
                    {
                        Heading = $"{questionnaire.Heading} > {subheadingQuestionnaire.Heading}",
                        QuestionId = subheadingQuestions.QuestionId,
                        Title = subheadingQuestions.Title,
                        Type = subheadingQuestions.Type,
                        Answer = string.Empty,
                        MultiSelectAnswerIds = [],
                        Rating = 0,
                        Answers = subheadingQuestions.Answers.Select(x => new McqAnswerDetails()
                        {
                            Id = x.Id, 
                            Title = x.Title
                        }).ToList()
                    });
                }
            }
        }
        
        return result;
    }
    #endregion

    #region Training Inspection Details
    private GetTrainingInspectionDetailsDto TrainingInspection { get; set; } = new();

    private async Task<GetTrainingInspectionDetailsDto> GetTrainingAndInspectionDetails(Guid trainingInspectionId)
    {
        try
        {
            var trainingInspection = await TrainingInspectionService.GetTrainingInspectionById(trainingInspectionId);

            if (trainingInspection?.Result is null)
            {
                SnackbarService.ShowSnackbar(trainingInspection?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                
                return new GetTrainingInspectionDetailsDto();
            }

            TrainingInspection = trainingInspection.Result;

            return TrainingInspection;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
        
        return new GetTrainingInspectionDetailsDto();
    }
    #endregion

    #region Training Details
    private GetTrainingDto Training { get; set; } = new();

    private async Task GetTrainingDetails(Guid trainingId)
    {
        try
        {
            var training = await TrainingService.GetTrainingById(trainingId);
            
            if (training?.Result is null)
            {
                SnackbarService.ShowSnackbar(training?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                
                return;
            }

            Training = training.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion

    #region Inspection Details
    private GetInspectionDto Inspection { get; set; } = new();

    private async Task GetInspectionDetails(Guid inspectionId)
    {
        try
        {
            var inspection = await InspectionService.GetInspectionById(inspectionId);
            
            if (inspection?.Result is null)
            {
                SnackbarService.ShowSnackbar(inspection?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                
                return;
            }

            Inspection = inspection.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    
    private bool IsInspectionModalOpen { get; set; }

    private void OpenCloseInspectionModal()
    {
        IsInspectionModalOpen = !IsInspectionModalOpen;
        
        StateHasChanged();
    }
    #endregion

    #region Candidate Details
    private UserDetail Candidate { get; set; } = new();
    
    private async Task GetCandidateDetails()
    {
        try
        {
            var result = await ProfileService.GetUserProfile();

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);

                return;
            }

            Candidate = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion

    #region Answer Upload
    private bool IsDisabled { get; set; }

    private string Remarks { get; set; } = "";
    
    private bool IsAnswerDetailsValid()
    {
        foreach (var questionDetail in QuestionDetails)
        {
            if (questionDetail.Type == QuestionType.SingleSelectMcq.ToString())
            {
                if (questionDetail.SingleSelectAnswerId == Guid.Empty)
                {
                    return false;
                }
            }
            else if (questionDetail.Type == QuestionType.MultiSelectMcq.ToString())
            {
                if (questionDetail.MultiSelectAnswerIds.Count == 0)
                {
                    return false;
                }
            }
            else if (questionDetail.Type == QuestionType.LongQuestion.ToString() || questionDetail.Type == QuestionType.ShortQuestion.ToString())
            {
                if (string.IsNullOrEmpty(questionDetail.Answer))
                {
                    return false;
                }
            }
            else if (questionDetail.Type == QuestionType.Rating.ToString())
            {
                if (questionDetail.Rating == 0)
                {
                    return false;
                }
            }
        }

        return true;
    }
    
    private void OnCheckedChanged(QuestionAnswerDetailsDto question, Guid answerId)
    {
        if (!question.MultiSelectAnswerIds.Contains(answerId))
        {
            question.MultiSelectAnswerIds.Add(answerId);
        }
        else
        {
            question.MultiSelectAnswerIds.Remove(answerId);
        }
        
        StateHasChanged();
    }
    
    private CandidateAnswerRequestDto MapQuestionnaireToCandidateAnswers(List<QuestionAnswerDetailsDto> questionnaireAnswers)
    {
        var candidateAnswers = new CandidateAnswerRequestDto
        {
            QuestionnaireId = Questionnaire.QuestionnaireId,
            Remarks = Remarks,
            Answers = []
        };

        foreach (var question in questionnaireAnswers)
        {
            var answerDetail = new AnswerDetailDto
            {
                QuestionId = question.QuestionId
            };

            if (question.Type == QuestionType.SingleSelectMcq.ToString())
            {
                answerDetail.AnswerId = new List<Guid>()
                {
                    question.SingleSelectAnswerId
                };
            }
            else if (question.Type == QuestionType.MultiSelectMcq.ToString())
            {
                answerDetail.AnswerId = question.MultiSelectAnswerIds.Select(x => x).ToList();
            }
            else if (question.Type == QuestionType.LongQuestion.ToString() || question.Type == QuestionType.ShortQuestion.ToString())
            {
                answerDetail.Title = question.Answer;
            }
            else if (question.Type == QuestionType.Rating.ToString())
            {
                answerDetail.Title = question.Rating.ToString();
            }

            candidateAnswers.Answers.Add(answerDetail);
        }

        return candidateAnswers;
    }
    
    private async Task OnAnswerSubmission()
    {
        try
        {
            IsDisabled = true;

            var answers = MapQuestionnaireToCandidateAnswers(QuestionDetails);

            var result = await AnswerService.UploadCandidateQuestionnaireAnswers(answers);

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