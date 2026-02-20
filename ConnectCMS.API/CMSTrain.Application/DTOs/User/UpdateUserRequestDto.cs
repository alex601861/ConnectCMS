using Microsoft.AspNetCore.Http;
using CMSTrain.Domain.Common.Enum;

namespace CMSTrain.Application.DTOs.User;

public class UpdateUserRequestDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public string EmailAddress { get; set; }
    
    public string PhoneNumber { get; set; }
    
    public GenderType Gender { get; set; }

    public string? Address { get; set; }
    
    public Guid? DesignationId { get; set; }
    
    public Guid? OrganizationId { get; set; }
    
    public Guid CountryId { get; set; }
    
    public IFormFile? Image { get; set; }
}