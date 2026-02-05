using CMSTrain.Domain.Common.Base;
using CMSTrain.Domain.Common.Enum;

namespace CMSTrain.Domain.Entities;

public class StrategicTrait : BaseEntity<Guid>
{
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public StrategicType Type { get; set; } 
}