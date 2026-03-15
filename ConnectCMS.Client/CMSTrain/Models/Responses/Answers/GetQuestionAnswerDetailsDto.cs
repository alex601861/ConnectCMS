namespace CMSTrain.Client.Models.Responses.Answers;

public class GetQuestionAnswerDetailsDto
{
    public Guid QuestionId { get; set; }

    public string Heading { get; set; }

    public string Title { get; set; }

    public string QuestionType { get; set; }

    public List<QuestionAnswerDetails> Answers { get; set; } = [];
}

public class QuestionAnswerDetails
{
    public Guid Id { get; set; }

    public int Rating { get; set; }

    public string Title { get; set; }

    public bool IsSelected { get; set; }

    public bool IsSelectable { get; set; }
}