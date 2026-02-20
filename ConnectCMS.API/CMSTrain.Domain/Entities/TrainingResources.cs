using CMSTrain.Domain.Common.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMSTrain.Domain.Entities;

public class TrainingResources : BaseEntity<Guid> 
{
    [ForeignKey(nameof(Training))]
    public Guid TrainingId { get; set; }

    [ForeignKey(nameof(Resource))]
    public Guid ResourceId { get; set; }
    
    public virtual Training? Training { get; set; }
    
    public virtual Resource? Resource { get; set; }
}