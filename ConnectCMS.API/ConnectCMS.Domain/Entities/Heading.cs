using CMSTrain.Domain.Common.Base;
using CMSTrain.Domain.Common.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMSTrain.Domain.Entities;

public class Heading : BaseEntity<Guid>
{
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public HeadingType Type { get; set; }
    
    public FacetType Facet { get; set; }
    
    public InspectionType Inspection { get; set; }
    
    [ForeignKey(nameof(ParentHeading))]
    public Guid? ParentHeadingId { get; set; }
    
    public virtual Heading ParentHeading { get; set; }
}