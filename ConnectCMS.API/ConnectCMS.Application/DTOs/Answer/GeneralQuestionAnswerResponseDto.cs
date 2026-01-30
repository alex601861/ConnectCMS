namespace CMSTrain.Application.DTOs.Answer;

public class GeneralQuestionAnswerResponseDto
{
    public Guid QuestionnaireId { get; set; }

    public Guid QuestionId { get; set; }
    
    public int Phase { get; set; }
    
    public string AnswerTitle { get; set; }
}