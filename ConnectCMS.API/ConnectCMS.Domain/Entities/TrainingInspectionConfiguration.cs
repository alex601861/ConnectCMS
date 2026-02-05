using CMSTrain.Domain.Common.Base;
using CMSTrain.Domain.Common.Property;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMSTrain.Domain.Entities;

public class TrainingInspectionConfiguration : BaseEntity<Guid>
{
    [ForeignKey(nameof(TrainingInspection))]
    public Guid TrainingInspectionId { get; set; }
    
    public KeyValueProperty PropertyPair { get; set; }
    
    public virtual TrainingInspection TrainingInspection { get; set; }
}