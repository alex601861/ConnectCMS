namespace CMSTrain.Client.Models.Responses.Answers;

public class GeneralQuestionnaireAnswerResponseDto
{
    public int TotalResponses { get; set; }

    public List<GeneralAnswerResponseDto> GeneralAnswers { get; set; } = [];
}