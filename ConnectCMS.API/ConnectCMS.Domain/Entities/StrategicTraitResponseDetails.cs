using CMSTrain.Domain.Common.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMSTrain.Domain.Entities;

public class StrategicTraitResponseDetails : BaseEntity<Guid>
{
    [ForeignKey(nameof(StrategicTraitResponse))]
    public Guid StrategicTraitResponseId { get; set; }
    
    [ForeignKey(nameof(StrategicTrait))]
    public Guid StrategicTraitId { get; set; }
    
    public virtual StrategicTrait? StrategicTrait { get; set; }
    
    public virtual StrategicTraitResponse? StrategicTraitResponse { get; set; }
}