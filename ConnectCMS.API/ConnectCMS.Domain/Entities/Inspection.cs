using CMSTrain.Domain.Common.Base;
using CMSTrain.Domain.Common.Enum;

namespace CMSTrain.Domain.Entities;

public class Inspection : BaseEntity<Guid>
{
    public string Name { get; set; }

    public string Description { get; set; }

    public InspectionType InspectionType { get; set; }
    
    public string ImageUrl { get; set; }
}