namespace CMSTrain.Client.Models.Requests.Answers;

public class QuestionAnswerDetailsDto
{
    public Guid QuestionId { get; set; }

    public string? Heading { get; set; }
    
    public string Title { get; set; }

    public int Rating { get; set; }
    
    public string? Answer { get; set; }

    public Guid SingleSelectAnswerId { get; set; }
    
    public List<Guid> MultiSelectAnswerIds { get; set; } = [];
    
    public string Type { get; set; }

    public List<McqAnswerDetails> Answers { get; set; } = [];
}

public class McqAnswerDetails
{
    public Guid Id { get; set; }

    public string Title { get; set; }
}