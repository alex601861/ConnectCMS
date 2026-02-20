using CMSTrain.Domain.Common.Base;

namespace CMSTrain.Domain.Entities;

public class TrainingFormat : BaseEntity<Guid>
{
    public string Name { get; set; }

    public string Description { get; set; }
}
