using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Extensions;
using CMSTrain.Client.Models.Responses.Heading;
using CMSTrain.Client.Models.Requests.Inspection;
using CMSTrain.Client.Models.Responses.Inspection;
using CMSTrain.Client.Models.Requests.Questionnaires;
using CMSTrain.Client.Models.Responses.Questionnaires;

namespace CMSTrain.Client.Pages.Administrator.Inspection;

public partial class InspectionQuestionnaire
{
    [Parameter] public InspectionType InspectionType { get; set; }

    [Parameter] public FacetType FacetType { get; set; }
    
    private bool IsLoading { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;
        
        await GetInspectionDetails();
        
        await GetAllHeadings();
        
        await GetAllSubHeadings();
        
        await GetInspectionQuestionnaire();
        
        SetPredefinedAnswersBasedOnInspection();
        
        IsLoading = false;
    }

    #region Inspection
    private GetInspectionDto Inspection { get; set; } = new();

    private async Task GetInspectionDetails()
    {
        var inspection = await InspectionService.GetInspectionByType(InspectionType);
            
        if (inspection?.Result is null)
        {
            SnackbarService.ShowSnackbar(inspection?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                
            return;
        }

        Inspection = inspection.Result;

        Inspection.ImageUrl = string.IsNullOrEmpty(Inspection.ImageUrl) 
            ? "images/dummy-img.png"
            : FileManager.FetchFileUrl(Inspection.ImageUrl, Constants.FilePath.InspectionImagesFilePath);
    }
    #endregion
    
    #region Inspection Questionnaire
    private GetQuestionnaireDto InspectionQuestionnaires { get; set; } = new(); 
    
    private async Task GetInspectionQuestionnaire()
    {
        try
        {
            var result = await QuestionnaireService.GetAllQuestionnairesFromInspectionUpload(Inspection.Id);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }
            
            InspectionQuestionnaires = result.Result;

            PredefinedAnswers = InspectionQuestionnaires.PredefinedAnswers;
            HasPredefinedAnswers = InspectionQuestionnaires.PredefinedAnswers.Count != 0;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }

    private void HandleQuestionTypeChange(GetQuestionDetailsDto question, string value)
    {
        var inspectionType = value == QuestionType.MultiSelectMcq.ToString() 
            ? QuestionType.MultiSelectMcq 
            : value == QuestionType.SingleSelectMcq.ToString() 
                ? QuestionType.SingleSelectMcq 
                : QuestionType.None;
        
        question.Type = value;

        if (inspectionType is not (QuestionType.MultiSelectMcq or QuestionType.SingleSelectMcq)) return;
        
        question.Answers = PredefinedAnswers.Where(x => x.QuestionType == inspectionType.ToString()).Select(x => new AnswerDetails()
        {
            Title = x.Title,
            IsSelectable = true
        }).ToList();
    }
    
    private void HandleQuestionTraitTypeChange(GetQuestionDetailsDto question, string value)
    {
        question.Trait = value;
    }
    #endregion
    
    #region Pre-defined Answers
    private bool HasPredefinedAnswers { get; set; }

    private List<McqAnswerDetailsDto> PredefinedAnswers { get; set; } = [];

    private List<QuestionType> PredefinedAnswersQuestionType { get; set; } = [ QuestionType.SingleSelectMcq, QuestionType.MultiSelectMcq ];
    
    private void SetPredefinedAnswers()
    {
        HasPredefinedAnswers = !HasPredefinedAnswers;
    }

    private void RemovePredefinedAnswers(QuestionType questionType)
    {
        PredefinedAnswers = PredefinedAnswers.Where(x => x.QuestionType != questionType.ToString()).ToList();

        if (PredefinedAnswers.Count == 0) SetPredefinedAnswers();
    }
    
    private void AddPredefinedAnswer(QuestionType questionType)
    {
        PredefinedAnswers.Add(new McqAnswerDetailsDto()
        {
            QuestionType = questionType.ToString()
        });
    }
    
    private void RemovePredefinedAnswer(McqAnswerDetailsDto answer)
    {
        PredefinedAnswers.Remove(answer);
    }
    
    private void SetPredefinedAnswersBasedOnInspection()
    {
        if (Inspection.Type.ToInspectionTypeString() != InspectionType.PersonalityTest) return;
        
        var predefinedAnswers = new List<McqAnswerDetailsDto>()
        {
            new()
            {
                Title = "Strongly Agree",
                QuestionType = QuestionType.SingleSelectMcq.ToString()
            },
            new()
            {
                Title = "Agree",
                QuestionType = QuestionType.SingleSelectMcq.ToString()
            },
            new()
            {
                Title = "Neutral",
                QuestionType = QuestionType.SingleSelectMcq.ToString()
            },
            new()
            {
                Title = "Disagree",
                QuestionType = QuestionType.SingleSelectMcq.ToString()
            },
            new()
            {
                Title = "Strongly Disagree",
                QuestionType = QuestionType.SingleSelectMcq.ToString()
            }
        };

        HasPredefinedAnswers = true;
            
        PredefinedAnswers = predefinedAnswers;

        PredefinedAnswersQuestionType = [ QuestionType.SingleSelectMcq ];
    }
    #endregion
    
    #region Upload Inspection Questionnaire
    private bool IsQuestionnaireValid()
    {
        if (ValidateQuestionnaire(InspectionQuestionnaires.Questions, InspectionType == InspectionType.PersonalityTest))
        {
            var headingQuestions = new List<GetQuestionDetailsDto>();

            foreach (var headingQuestionDetails in InspectionQuestionnaires.HeadingQuestions)
            {
                headingQuestions.AddRange(headingQuestionDetails.Questions);
    
                foreach (var subHeading in headingQuestionDetails.SubHeadingQuestions)
                {
                    headingQuestions.AddRange(subHeading.Questions);
                }
            }

            if (ValidateQuestionnaire(headingQuestions, InspectionType == InspectionType.PersonalityTest))
            {
                return true;
            }

            return false;
        }

        return false;
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
    
    private UploadInspectionQuestionnaireDto UploadInspectionQuestionnaireDto { get; set; } = new();
    
    private async Task UploadInspectionQuestionnaire()
    {
        try
        {
            UploadInspectionQuestionnaireDto = new UploadInspectionQuestionnaireDto()
            {
                InspectionId = Inspection.Id,
                RequiresPredefinedAnswers = Inspection.Type.ToInspectionTypeString() != InspectionType.PersonalityTest && HasPredefinedAnswers,
                Answers = Inspection.Type.ToInspectionTypeString() != InspectionType.PersonalityTest ? PredefinedAnswers : [],
                QuestionnaireDetails = BuildQuestionnaireDetails()
            };
            
            var result = await InspectionService.UploadInspectionQuestionnaires(UploadInspectionQuestionnaireDto);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }
            
            SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
            
            NavigationManager.NavigateTo("/inspections");
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    
    private List<QuestionDetailsDto> BuildQuestionnaireDetails()
    {
        var questions = new List<QuestionDetailsDto>();

        AddQuestions(InspectionQuestionnaires.Questions, questions, null, null);

        foreach (var headingQuestion in InspectionQuestionnaires.HeadingQuestions)
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
                Title = x.Title
            }).ToList() : 
                question.Answers.Select(x => new McqAnswerDetailsDto
                {
                    Title = x.Title
                }).ToList()
        }));
    }
    #endregion
    
    #region Headings and Sub-Headings
    private List<GetHeadingModuleDto> Headings { get; set; } = [];

    private List<GetHeadingModuleDto> SubHeadings { get; set; } = [];

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
        InspectionQuestionnaires.Questions = [];
        InspectionQuestionnaires.HeadingQuestions = [];
    }
    #endregion
    
    #region Question Manipulation
    #region Normal Questions (without Headings)
    private void AddQuestion()
    {
        InspectionQuestionnaires.Questions.Add(new GetQuestionDetailsDto());    
    }
    
    private void RemoveQuestion(GetQuestionDetailsDto question)
    {
        InspectionQuestionnaires.Questions.Remove(question);    
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
                InspectionQuestionnaires.HeadingQuestions.Add(new GetHeadingQuestionDetailsDto()
                {
                    HeadingId = heading.Id,
                    Heading = heading.Title
                }); 
            }
        }

        OpenCloseAddHeadingModal();
    }
    
    private void RemoveHeading(GetHeadingQuestionDetailsDto heading)
    {
        InspectionQuestionnaires.HeadingQuestions.Remove(heading);    
    }
    
    private void AddQuestionToHeading(GetHeadingQuestionDetailsDto headingQuestion)
    {
        headingQuestion.Questions.Add(new GetQuestionDetailsDto());    
    }
    
    private void RemoveQuestionFromHeading(GetHeadingQuestionDetailsDto heading, GetQuestionDetailsDto headingQuestion)
    {
        heading.Questions.Remove(headingQuestion);  
        
        if (heading.Questions.Count == 0 && heading.SubHeadingQuestions.Count == 0)
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
}