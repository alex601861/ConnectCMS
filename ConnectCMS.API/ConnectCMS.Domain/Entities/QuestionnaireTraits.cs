using CMSTrain.Domain.Common.Base;
using CMSTrain.Domain.Common.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMSTrain.Domain.Entities;

public class QuestionnaireTraits : BaseEntity<Guid>
{
    [ForeignKey(nameof(Question))]
    public Guid QuestionId { get; set; }
    
    public TraitType TraitType { get; set; }
    
    public virtual QuestionnaireDetails? Question { get; set; }
}