using System.ComponentModel.DataAnnotations.Schema;
using CMSTrain.Domain.Common.Base;
using CMSTrain.Domain.Common.Enum;

namespace CMSTrain.Domain.Entities;

public class StrategicTraitDetails : BaseEntity<Guid>
{
    [ForeignKey(nameof(Trait))]
    public Guid TraitId { get; set; }
    
    [ForeignKey(nameof(Detail))]
    public Guid DetailId { get; set; }
    
    public virtual StrategicTrait? Trait { get; set; }
    
    public virtual StrategicTrait? Detail { get; set; }
}