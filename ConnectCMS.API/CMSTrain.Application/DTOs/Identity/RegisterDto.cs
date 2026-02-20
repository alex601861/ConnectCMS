using CMSTrain.Application.Common.Attributes;
using CMSTrain.Domain.Common.Enum;
using Microsoft.AspNetCore.Http;

namespace CMSTrain.Application.DTOs.Identity;

public class RegisterDto
{
    public string Name { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public string ConfirmPassword { get; set; }

    public string? PhoneNumber { get; set; }
    
    public GenderType Gender { get; set; }

    public string? Address { get; set; }
    
    public Guid? DesignationId { get; set; }
    
    public Guid? CountryId { get; set; }

    [FileExamination(5 * 1024 * 1024, true)]
    public IFormFile? ImageUrl { get; set; }
}
