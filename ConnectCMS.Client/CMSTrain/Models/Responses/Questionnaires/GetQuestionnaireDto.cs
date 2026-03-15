using CMSTrain.Client.Models.Requests.Questionnaires;

namespace CMSTrain.Client.Models.Responses.Questionnaires;

public class GetQuestionnaireDto
{
    public Guid QuestionnaireId { get; set; }
    
    public bool IsQuestionnaireForTraining { get; set; }
    
    public Guid? TrainingInspectionId { get; set; }

    public List<McqAnswerDetailsDto> PredefinedAnswers { get; set; } = [];

    public List<GetHeadingQuestionDetailsDto> HeadingQuestions { get; set; } = [];
    
    public List<GetQuestionDetailsDto> Questions { get; set; } = [];
}