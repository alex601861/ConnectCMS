using CMSTrain.Client.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace CMSTrain.Client.Models.Requests.Identity;

public class ProfileRequestDto
{
    [Required(ErrorMessage = "Please enter your name.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Please enter your email address.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Please enter your phone number.")]
    public string PhoneNumber { get; set; }

    [Required(ErrorMessage = "Please select your country.")]
    public Guid CountryId { get; set; }
    
    public string? Address { get; set; }

    public Guid? DesignationId { get; set; } = Guid.Empty;
    
    [Required(ErrorMessage = "Please select your gender.")]
    public GenderType Gender { get; set; }
}