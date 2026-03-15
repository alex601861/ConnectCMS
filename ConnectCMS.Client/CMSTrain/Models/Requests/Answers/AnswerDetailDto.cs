namespace CMSTrain.Client.Models.Requests.Answers;

public class AnswerDetailDto
{
    public Guid QuestionId { get; set; }

    public List<Guid>? AnswerId { get; set; }

    public string? Title { get; set; }
}
