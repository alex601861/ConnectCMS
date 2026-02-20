namespace CMSTrain.Application.DTOs.Answer;

public class AnswerDetailDto
{
    public Guid QuestionId { get; set; }

    public List<Guid>? AnswerId { get; set; }

    public string? Title { get; set; }
}
