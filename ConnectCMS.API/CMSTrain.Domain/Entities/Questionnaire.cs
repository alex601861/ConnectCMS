using CMSTrain.Domain.Common.Base;
using System.ComponentModel.DataAnnotations.Schema;
using CMSTrain.Domain.Common.Enum;

namespace CMSTrain.Domain.Entities;

public class Questionnaire : BaseEntity<Guid>
{
    public bool IsQuestionnaireForTraining { get; set; }
    
    [ForeignKey(nameof(TrainingInspection))]
    public Guid? TrainingInspectionId { get; set; }
    
    public virtual TrainingInspection? TrainingInspection { get; set; }

    public virtual ICollection<QuestionnaireDetails>? QuestionDetails { get; set; }
}
