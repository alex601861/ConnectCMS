using CMSTrain.Client.Models.Constants;

namespace CMSTrain.Client.Models.Requests.Questionnaires;

public class QuestionDetailsDto
{
    public string Title { get; set; }

    public Guid? HeadingId { get; set; }
    
    public bool? IsParentHeading { get; set; }

    public QuestionType Type { get; set; } = QuestionType.LongQuestion;
    
    public List<TraitType>? TraitTypes { get; set; } = [];

    public List<McqAnswerDetailsDto> Answers { get; set; } = [];
}