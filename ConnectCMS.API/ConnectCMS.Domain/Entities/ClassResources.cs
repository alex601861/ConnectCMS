using System.ComponentModel.DataAnnotations.Schema;
using CMSTrain.Domain.Common.Base;

namespace CMSTrain.Domain.Entities;

public class ClassResources : BaseEntity<Guid>
{
    [ForeignKey(nameof(Class))]
    public Guid ClassId { get; set; }

    [ForeignKey(nameof(Resource))]
    public Guid ResourceId { get; set; }
    
    public virtual Class? Class { get; set; }
    
    public virtual Resource? Resource { get; set; }
}