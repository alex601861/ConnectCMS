using CMSTrain.Domain.Common.Base;
using CMSTrain.Domain.Common.Enum;

namespace CMSTrain.Domain.Entities;

public class PersonalityTrait : BaseEntity<Guid>
{
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public TraitType Type { get; set; }
}