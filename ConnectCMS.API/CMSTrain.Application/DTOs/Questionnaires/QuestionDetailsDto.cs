using CMSTrain.Domain.Common.Enum;

namespace CMSTrain.Application.DTOs.Questionnaires;

// Question Details to be uploaded via Excel or Forms
public class QuestionDetailsDto
{
    public string Title { get; set; }

    public Guid? HeadingId { get; set; }
    
    public bool? IsParentHeading { get; set; }
    
    public QuestionType Type { get; set; }
    
    public List<TraitType>? TraitTypes { get; set; }

    public List<McqAnswerDetailsDto> Answers { get; set; }
}