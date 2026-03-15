using CMSTrain.Client.Models.Constants;
using Microsoft.AspNetCore.Components.Forms;

namespace CMSTrain.Client.Models.Requests.User;

public class UpdateUserRequestDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public string EmailAddress { get; set; }

    public string PhoneNumber { get; set; }

    public GenderType? Gender { get; set; }
    
    public Guid CountryId { get; set; }
    
    public string? ImageUrl { get; set; }

    public Guid RoleId { get; set; }

    public string? Address { get; set; }

    public Guid? DesignationId { get; set; } = Guid.Empty;
    
    public Guid? OrganizationId { get; set; } = Guid.Empty;
    
    public string? Organization { get; set; }
    
    public string Role { get; set; }
    
    public IBrowserFile? Image { get; set; }
}