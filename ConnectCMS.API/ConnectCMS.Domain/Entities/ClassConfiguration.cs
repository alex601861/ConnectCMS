using CMSTrain.Domain.Common.Base;
using CMSTrain.Domain.Common.Property;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMSTrain.Domain.Entities;

public class ClassConfiguration : BaseEntity<Guid>
{
    [ForeignKey(nameof(Class))]
    public Guid ClassId { get; set; }
    
    public KeyValueProperty PropertyPair { get; set; }
    
    public virtual Class Class { get; set; }
}