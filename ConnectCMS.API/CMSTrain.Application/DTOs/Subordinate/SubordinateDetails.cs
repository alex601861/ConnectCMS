using CMSTrain.Domain.Common.Enum;

namespace CMSTrain.Application.DTOs.Subordinate;

public class SubordinateDetails
{
    public string Name { get; set; }

    public string Email { get; set; }

    public string ContactNumber { get; set; }

    public SubordinateType Type { get; set;}
}