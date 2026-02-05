using CMSTrain.Domain.Common.Base;

namespace CMSTrain.Domain.Entities;

public class Organization : BaseEntity<Guid>
{
    public string Name { get; set; }

    public string Address { get; set; }

    public string Description { get; set; }

    public string ImageUrl { get; set; }
}
