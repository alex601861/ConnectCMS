using System.ComponentModel.DataAnnotations.Schema;
using CMSTrain.Domain.Common.Base;

namespace CMSTrain.Domain.Entities;

public class TrainingInspection : BaseEntity<Guid>
{
    [ForeignKey(nameof(Training))]
    public Guid TrainingId { get; set; }
    
    [ForeignKey(nameof(Inspection))]
    public Guid InspectionId { get; set; }
    
    public virtual Training Training { get; set; }
    
    public virtual Inspection Inspection { get; set; }
}