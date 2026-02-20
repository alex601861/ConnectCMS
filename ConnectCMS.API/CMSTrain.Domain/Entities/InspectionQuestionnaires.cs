using CMSTrain.Domain.Common.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMSTrain.Domain.Entities;

public class InspectionQuestionnaires : BaseEntity<Guid>
{
    [ForeignKey(nameof(Inspection))]
    public Guid InspectionId { get; set; }
    
    [ForeignKey(nameof(Questionnaire))]
    public Guid QuestionnaireId { get; set; }
    
    public virtual Inspection? Inspection { get; set; }

    public virtual Questionnaire? Questionnaire { get; set; }
}