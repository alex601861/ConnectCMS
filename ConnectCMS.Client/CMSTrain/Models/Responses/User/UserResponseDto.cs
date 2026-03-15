namespace CMSTrain.Client.Models.Responses.User;

public class UserResponseDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string PhoneNumber { get; set; }

    public string? ImageUrl { get; set; }

    public string UserName { get; set; }

    public string Gender { get; set; }
    
    public string Email { get; set; }

    public Guid RoleId { get; set; }

    public string Role { get; set; }

    public Guid CountryId { get; set; }

    public string Country { get; set; }

    public Guid? OrganizationId { get; set; }

    public string? Organization { get; set; }
    
    public Guid? DesignationId { get; set; }
    
    public string? Designation { get; set; }

    public bool IsActive { get; set; }
}