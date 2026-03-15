using CMSTrain.Client.Models.Constants;

namespace CMSTrain.Client.Models.Responses.Questionnaires;

public class GetQuestionDetailsDto
{
    public Guid QuestionId { get; set; }

    public string Heading { get; set; }
    
    public int Rating { get; set; } = 0;
    
    public string Title { get; set; }

    public string Type { get; set; } = QuestionType.None.ToString();

    public string Trait { get; set; } = TraitType.None.ToString();

    public List<AnswerDetails> Answers { get; set; } = [];
}

public class AnswerDetails
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public bool IsSelectable { get; set; }
}