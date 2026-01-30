namespace CMSTrain.Application.DTOs.Answer;

public class GeneralQuestionnaireAnswerResponseDto
{
    public int TotalResponses { get; set; }
    
    public List<GeneralAnswerResponseDto> GeneralAnswers { get; set; }
}