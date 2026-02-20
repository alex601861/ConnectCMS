using CMSTrain.Domain.Common.Enum;

namespace CMSTrain.Application.DTOs.Heading;

public class CreateHeadingDto
{
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public HeadingType Type { get; set; }
    
    public FacetType Facet { get; set; }

    public InspectionType Inspection { get; set; }
    
    public Guid? ParentHeadingId { get; set; }
}