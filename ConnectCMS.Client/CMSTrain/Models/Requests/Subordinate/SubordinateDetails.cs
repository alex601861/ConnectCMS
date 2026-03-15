using CMSTrain.Client.Models.Constants;

namespace CMSTrain.Client.Models.Requests.Subordinate;

public class SubordinateDetails
{
    public string Name { get; set; }

    public string Email { get; set; }

    public string ContactNumber { get; set; }

    public SubordinateType Type { get; set; } = SubordinateType.Junior;
}