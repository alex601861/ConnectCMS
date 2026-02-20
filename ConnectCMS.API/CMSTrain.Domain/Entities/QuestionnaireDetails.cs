using CMSTrain.Domain.Common.Base;
using CMSTrain.Domain.Common.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMSTrain.Domain.Entities;

public class QuestionnaireDetails : BaseEntity<Guid>
{
    [ForeignKey(nameof(Questionnaire))]
    public Guid QuestionnaireId { get; set; }

    public string Title { get; set; } = "";

    [ForeignKey(nameof(Heading))]
    public Guid? HeadingId { get; set; } 
    
    public bool? IsParentHeading { get; set; }
    
    public bool HasUniqueAnswers { get; set; }
    
    public int Order { get; set; }
    
    public QuestionType QuestionType { get; set; } 

    public virtual Questionnaire? Questionnaire { get; set; }

    public virtual Heading? Heading { get; set; }
    
    public virtual ICollection<Answer>? Answers { get; set; }

    public virtual ICollection<QuestionnaireTraits>? QuestionnaireTraits { get; set; }
}
