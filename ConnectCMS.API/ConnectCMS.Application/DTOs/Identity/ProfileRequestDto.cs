using CMSTrain.Domain.Common.Enum;

namespace CMSTrain.Application.DTOs.Identity;

public class ProfileRequestDto
{
    public string Name { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public Guid CountryId { get; set; }
    
    public string Address { get; set; }
    
    public Guid? DesignationId { get; set; }
    
    public GenderType Gender { get; set; }
}
