using CMSTrain.Client.Models.Constants;

namespace CMSTrain.Client.Models.Requests.Heading;

public class CreateHeadingDto
{
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public HeadingType Type { get; set; }

    public FacetType Facet { get; set; }

    public InspectionType Inspection { get; set; }
    
    public Guid? ParentHeadingId { get; set; }
}