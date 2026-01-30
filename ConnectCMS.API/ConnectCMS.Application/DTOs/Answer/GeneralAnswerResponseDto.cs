namespace CMSTrain.Application.DTOs.Answer;

public class GeneralAnswerResponseDto
{
    public Guid QuestionId { get; set; }

    public string Title { get; set; }

    public string QuestionType { get; set; }
    
    public List<AnswerResponseDetails> Answers { get; set; }
}

public class AnswerResponseDetails
{
    public Guid AnswerId { get; set; }
    
    public string Answer { get; set; }
    
    public double Count { get; set; }
    
    public double Percentage { get; set; }
}