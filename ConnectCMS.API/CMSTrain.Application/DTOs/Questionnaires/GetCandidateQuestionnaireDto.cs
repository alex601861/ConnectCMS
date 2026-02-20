namespace CMSTrain.Application.DTOs.Questionnaires;

public class GetCandidateQuestionnaireDto
{
    public Guid QuestionnaireId { get; set; }

    public Guid TrainingInspectionId { get; set; }
    
    public List<HeadingQuestionDetailsDto> HeadingQuestions { get; set; } = [];
    
    public List<GetQuestionDetailsDto> Questions { get; set; } = [];
}