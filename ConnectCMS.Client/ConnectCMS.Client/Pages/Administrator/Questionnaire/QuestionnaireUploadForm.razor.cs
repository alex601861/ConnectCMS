using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Service.Extensions;
using CMSTrain.Client.Models.Responses.Heading;
using CMSTrain.Client.Models.Responses.Training;
using CMSTrain.Client.Models.Responses.Inspection;
using CMSTrain.Client.Models.Requests.Questionnaires;
using CMSTrain.Client.Models.Responses.Questionnaires;
using CMSTrain.Client.Models.Responses.TrainingInspection;

namespace CMSTrain.Client.Pages.Administrator.Questionnaire;

public partial class QuestionnaireUploadForm : ComponentBase
{
    [Parameter] public Guid TrainingInspectionId { get; set; }

    private FacetType FacetType { get; set; } = FacetType.Facet;
    
    private InspectionType InspectionType { get; set; } = InspectionType.PersonalityTest;
    
    private bool IsLoading { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();

        IsLoading = true;
        
        var trainingInspection = await GetTrainingAndInspectionDetails();

        await GetTrainingDetails(trainingInspection.TrainingId);

        await GetInspectionDetails(trainingInspection.InspectionId);

        MapInspectionCategories();
        
        await GetAllHeadings();
        
        await GetAllSubHeadings();
        
        await GetQuestionnaireDetails();
        
        SetPredefinedAnswersBasedOnInspection();

        IsLoading = false;
    }

    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.Questionnaire;
    }
    #endregion

    #region Training Inspection
    private GetTrainingInspectionDetailsDto TrainingInspection { get; set; } = new();

    private async Task<GetTrainingInspectionDetailsDto> GetTrainingAndInspectionDetails()
    {
        try
        {
            var trainingInspection = await TrainingInspectionService.GetTrainingInspectionById(TrainingInspectionId);

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
        var training = await TrainingService.GetTrainingById(trainingId);
            
        if (training?.Result is null)
        {
            SnackbarService.ShowSnackbar(training?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                
            return;
        }

        Training = training.Result;
        
        Training.ImageUrl = string.IsNullOrEmpty(Training.ImageUrl) 
            ? "images/dummy-img.png" 
            : FileManager.FetchFileUrl(Training.ImageUrl, Constants.FilePath.TrainingsImagesFilePath);
    }
    #endregion

    #region Inspection Details
    private GetInspectionDto Inspection { get; set; } = new();

    private async Task GetInspectionDetails(Guid inspectionId)
    {
        var inspection = await InspectionService.GetInspectionById(inspectionId);
            
        if (inspection?.Result is null)
        {
            SnackbarService.ShowSnackbar(inspection?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                
            return;
        }

        Inspection = inspection.Result;
    }
    #endregion

    #region Questionnaire Details and Upload (from Inspection)
    private GetQuestionnaireDto Questionnaire { get; set; } = new();

    private async Task GetQuestionnaireDetails()
    {
        try
        {
            var result = await QuestionnaireService.GetAllQuestionnairesForTrainingInspection(TrainingInspectionId);
            
            if(result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            Questionnaire = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    
    private QuestionnaireUploadDto QuestionnaireUpload { get; set; } = new();

    private async Task OnQuestionnaireUpload()
    {
        try
        {
            QuestionnaireUpload = new QuestionnaireUploadDto()
            {
                TrainingInspectionId = TrainingInspection.TrainingInspectionId,
                QuestionDetails = BuildQuestionnaireDetails()
            };
            
            var result = await QuestionnaireService.UploadQuestionnaires(QuestionnaireUpload);
            
            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }
            
            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    NavigationManager.NavigateTo($"trainings/admin/training-details/{Training.Id}");
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
    }
    #endregion

    #region Questionnaire Details and Upload
    private bool IsQuestionnaireValid()
    {
        // if (ValidateQuestionnaire(Questionnaire.Questions, InspectionType == InspectionType.PersonalityTest))
        // {
        //     var headingQuestions = new List<GetQuestionDetailsDto>();
        //
        //     foreach (var headingQuestionDetails in Questionnaire.HeadingQuestions)
        //     {
        //         headingQuestions.AddRange(headingQuestionDetails.Questions);
        //
        //         foreach (var subHeading in headingQuestionDetails.SubHeadingQuestions)
        //         {
        //             headingQuestions.AddRange(subHeading.Questions);
        //         }
        //     }
        //
        //     if (ValidateQuestionnaire(headingQuestions, InspectionType == InspectionType.PersonalityTest))
        //     {
        //         return true;
        //     }
        // }
        //
        // return false;

        return true;
    }

    private bool ValidateQuestionnaire(List<GetQuestionDetailsDto> questionDetails, bool isTraitRequired)
    {
        foreach (var question in questionDetails)
        {
            if (string.IsNullOrEmpty(question.Title) || string.IsNullOrEmpty(question.Type)) return false;

            if (question.Type == QuestionType.None.ToString()) return false;

            if (isTraitRequired)
            {
                if (question.Trait == TraitType.None.ToString()) return false;   
            }
            
            if (question.Type == QuestionType.SingleSelectMcq.ToString() || question.Type == QuestionType.MultiSelectMcq.ToString())
            {
                foreach (var answer in question.Answers)
                {
                    if (string.IsNullOrEmpty(answer.Title)) return false;
                }
            }
        }

        return true;
    }
    
    private List<QuestionDetailsDto> BuildQuestionnaireDetails()
    {
        var questions = new List<QuestionDetailsDto>();

        AddQuestions(Questionnaire.Questions, questions, null, null);

        foreach (var headingQuestion in Questionnaire.HeadingQuestions)
        {
            AddQuestions(headingQuestion.Questions, questions, headingQuestion.HeadingId, true);

            foreach (var subHeadingQuestion in headingQuestion.SubHeadingQuestions)
            {
                AddQuestions(subHeadingQuestion.Questions, questions, subHeadingQuestion.HeadingId, false);
            }
        }

        return questions;
    }
    
    private void AddQuestions(IEnumerable<GetQuestionDetailsDto> sourceQuestions, List<QuestionDetailsDto> questions, Guid? headingId, bool? isParentHeading)
    {
        questions.AddRange(sourceQuestions.Select(question => new QuestionDetailsDto
        {
            Title = question.Title,
            HeadingId = headingId,
            IsParentHeading = isParentHeading,
            Type = Inspection.Type.ToInspectionTypeString() == InspectionType.PersonalityTest ? QuestionType.SingleSelectMcq : Enum.TryParse(question.Type, out QuestionType questionType) ? questionType : QuestionType.LongQuestion,
            TraitTypes = [ Enum.TryParse(question.Trait, out TraitType traitType) ? traitType : TraitType.Openness ],
            Answers = Inspection.Type.ToInspectionTypeString() == InspectionType.PersonalityTest ? PredefinedAnswers.Select(x => new McqAnswerDetailsDto 
                {
                    Title = x.Title, 
                }).ToList() : 
                question.Answers.Select(x => new McqAnswerDetailsDto
                {
                    Title = x.Title, 
                }).ToList()
        }));
    }
    #endregion

    #region Headings and Sub-Headings
    private List<GetHeadingModuleDto> Headings { get; set; } = [];

    private List<GetHeadingModuleDto> SubHeadings { get; set; } = [];

    private void MapInspectionCategories()
    {
        if (Inspection.Type.ToInspectionTypeString() == InspectionType.Feedback)
        {
            FacetType = FacetType.Division;
            InspectionType = InspectionType.Feedback;
        }
        else if (Inspection.Type.ToInspectionTypeString() == InspectionType.PersonalAssessment)
        {
            FacetType = FacetType.Heading;
            InspectionType = InspectionType.PersonalAssessment;
        }
        else if (Inspection.Type.ToInspectionTypeString() == InspectionType.PersonalityTest)
        {
            FacetType = FacetType.Facet;
            InspectionType = InspectionType.PersonalityTest;
        }
    }
    
    private async Task GetAllHeadings()
    {
        try
        {
            var result = await HeadingService.GetAllParentHeadings(FacetType, InspectionType);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }
            
            Headings = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }

    private async Task GetAllSubHeadings()
    {
        try
        {
            var result = await HeadingService.GetAllSubHeadings();

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }
            
            SubHeadings = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    
    private List<GetHeadingModuleDto> GetSubHeadingsByParentId(Guid parentId)
    {
        return SubHeadings.Where(x => x.ParentHeadingId == parentId).ToList();
    }
    #endregion
    
    #region Clear Questionnaire Form
    private void ClearQuestionnaireForm()
    {
        Questionnaire.Questions = [];
        Questionnaire.HeadingQuestions = [];
    }
    #endregion

    #region Question Manipulation
    #region Normal Questions (without Headings)
    private void AddQuestion()
    {
        Questionnaire.Questions.Add(new GetQuestionDetailsDto());    
    }
    
    private void RemoveQuestion(GetQuestionDetailsDto question)
    {
        Questionnaire.Questions.Remove(question);    
    }
    
    private static void AddAnswers(GetQuestionDetailsDto questionDetails)
    {
        questionDetails.Answers.Add(new AnswerDetails());
    }
    
    private static void RemoveAnswers(GetQuestionDetailsDto questionDetails, AnswerDetails answerDetails)
    {
        questionDetails.Answers.Remove(answerDetails);
    }
    #endregion

    #region Questions with Headings
    private void RemoveHeading(GetHeadingQuestionDetailsDto heading)
    {
        Questionnaire.HeadingQuestions.Remove(heading);    
    }
    
    private void AddQuestionToHeading(GetHeadingQuestionDetailsDto headingQuestion)
    {
        headingQuestion.Questions.Add(new GetQuestionDetailsDto());    
    }
    
    private void RemoveQuestionFromHeading(GetHeadingQuestionDetailsDto heading, GetQuestionDetailsDto headingQuestion)
    {
        heading.Questions.Remove(headingQuestion);    
        
        if (heading.Questions.Count == 0)
        {
            RemoveHeading(heading);
        }
    }
    
    private void AddAnswerToHeadingQuestion(GetQuestionDetailsDto headingQuestion)
    {
        headingQuestion.Answers.Add(new AnswerDetails());    
    }
    
    private void RemoveAnswerFromHeadingQuestion(GetQuestionDetailsDto headingQuestion, AnswerDetails answer)
    {
        headingQuestion.Answers.Remove(answer);    
    }
    #endregion

    #region Questions with Sub Headings
    private void RemoveSubHeadingFromHeading(GetHeadingQuestionDetailsDto heading, SubHeadingDto subHeading)
    {
        heading.SubHeadingQuestions.Remove(subHeading);    
    }
    
    private void AddQuestionToSubHeading(SubHeadingDto subHeading)
    {
        subHeading.Questions.Add(new GetQuestionDetailsDto());    
    }
    
    private void RemoveQuestionFromSubHeading(SubHeadingDto subHeading, GetQuestionDetailsDto subHeadingQuestion)
    {
        subHeading.Questions.Remove(subHeadingQuestion);    
        
        if (subHeading.Questions.Count == 0)
        {
            RemoveSubHeadingFromHeading(SelectedHeading, subHeading);
        }
    }
    
    private void AddAnswerToSubHeadingQuestion(GetQuestionDetailsDto subHeadingQuestion)
    {
        subHeadingQuestion.Answers.Add(new AnswerDetails());    
    }
    
    private void RemoveAnswerFromSubHeading(GetQuestionDetailsDto subHeading, AnswerDetails subHeadingAnswer)
    {
        subHeading.Answers.Remove(subHeadingAnswer);    
    }
    #endregion    
    #endregion

    #region Map Question Type
    private void HandleQuestionTypeChange(GetQuestionDetailsDto question, string value)
    {
        question.Type = value;
    }
    
    private void HandleQuestionTraitTypeChange(GetQuestionDetailsDto question, string value)
    {
        question.Trait = value;
    }
    #endregion

    #region Heading and Subheading
    private bool IsAddHeadingModalOpen { get; set; }
    
    private void OpenCloseAddHeadingModal()
    {
        HeadingId = Guid.Empty;
        
        IsAddHeadingModalOpen = !IsAddHeadingModalOpen;
        
        StateHasChanged();
    }
    
    private Guid HeadingId { get; set; }
    
    private void AddHeading(bool isClosed)
    {
        if (!isClosed)
        {
            var heading = Headings.FirstOrDefault(x => x.Id == HeadingId);

            if (heading != null)
            {
                Questionnaire.HeadingQuestions.Add(new GetHeadingQuestionDetailsDto()
                {
                    HeadingId = heading.Id,
                    Heading = heading.Title
                }); 
            }
        }

        OpenCloseAddHeadingModal();
    }
    
    private bool IsAddSubHeadingModalOpen { get; set; }

    private GetHeadingQuestionDetailsDto SelectedHeading { get; set; } = new();
    
    private void OpenCloseAddSubHeadingModal(GetHeadingQuestionDetailsDto selectedHeading)
    {
        SubHeadingId = Guid.Empty;

        SelectedHeading = selectedHeading;
        
        IsAddSubHeadingModalOpen = !IsAddSubHeadingModalOpen;
        
        StateHasChanged();
    }
    
    private Guid SubHeadingId { get; set; }
    
    private void AddSubHeading(bool isClosed)
    {
        if (!isClosed)
        {
            var subHeading = SubHeadings.FirstOrDefault(x => x.Id == SubHeadingId);

            if (subHeading != null)
            {
                SelectedHeading.SubHeadingQuestions.Add(new SubHeadingDto()
                {
                    HeadingId = subHeading.Id,
                    Heading = subHeading.Title
                });
            }
        }

        OpenCloseAddSubHeadingModal(new());
    }
    #endregion

    #region Personality Test 
    private List<McqAnswerDetailsDto> PredefinedAnswers { get; set; } = [];

    private void SetPredefinedAnswersBasedOnInspection()
    {
        if (Inspection.Type.ToInspectionTypeString() == InspectionType.PersonalityTest)
        {
            var predefinedAnswers = new List<McqAnswerDetailsDto>()
            {
                new McqAnswerDetailsDto()
                {
                    Title = "Strongly Agree",
                },
                new McqAnswerDetailsDto()
                {
                    Title = "Agree",
                },
                new McqAnswerDetailsDto()
                {
                    Title = "Neutral",
                },
                new McqAnswerDetailsDto()
                {
                    Title = "Disagree",
                },
                new McqAnswerDetailsDto()
                {
                    Title = "Strongly Disagree",
                }
            };

            PredefinedAnswers = predefinedAnswers;
        }
    }    
    #endregion
}