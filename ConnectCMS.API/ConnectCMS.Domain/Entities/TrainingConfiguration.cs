using CMSTrain.Domain.Common.Base;
using CMSTrain.Domain.Common.Property;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMSTrain.Domain.Entities;

public class TrainingConfiguration : BaseEntity<Guid>
{
    [ForeignKey(nameof(Training))]
    public Guid TrainingId { get; set; }
    
    public KeyValueProperty PropertyPair { get; set; }
    
    public virtual Training Training { get; set; }
}