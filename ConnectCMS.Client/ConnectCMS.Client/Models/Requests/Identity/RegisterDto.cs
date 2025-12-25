using CMSTrain.Client.Models.Constants;
using Microsoft.AspNetCore.Components.Forms;

namespace CMSTrain.Client.Models.Requests.Identity;

public class RegisterDto
{
    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? ConfirmPassword { get; set; }

    public string? PhoneNumber { get; set; }
    
    public GenderType? Gender { get; set; }
    
    public string? Address { get; set; }

    public Guid? DesignationId { get; set; } = Guid.Empty;

    public Guid? CountryId { get; set; } = Guid.Empty;

    public IBrowserFile? ImageUrl { get; set; }
}
