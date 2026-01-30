namespace CMSTrain.Application.DTOs.Questionnaires;

// Get questionnaire (with question details) for Candidate View
public class GetQuestionnaireDto
{
    public Guid QuestionnaireId { get; set; }
    
    public bool IsQuestionnaireForTraining { get; set; }
    
    public Guid? TrainingInspectionId { get; set; }
    
    public List<McqAnswerDetailsDto> PredefinedAnswers { get; set; } = [];

    public List<HeadingQuestionDetailsDto> HeadingQuestions { get; set; } = [];
    
    public List<GetQuestionDetailsDto> Questions { get; set; } = [];
}


