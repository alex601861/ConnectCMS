namespace CMSTrain.Application.DTOs.Questionnaires;

// Get all questions (question details) for Candidate and Trainer View
public class GetQuestionDetailsDto
{
    public Guid QuestionId { get; set; }

    public string Heading { get; set; }
    
    public int Rating { get; set; } = 0;

    public string Title { get; set; }

    public string Type { get; set; }

    public string Trait { get; set; }

    public List<AnswerDetails> Answers { get; set; }
}

public class AnswerDetails
{
    public Guid Id { get; set; }

    public string Title { get; set; }
    
    public bool IsSelectable { get; set; }
}