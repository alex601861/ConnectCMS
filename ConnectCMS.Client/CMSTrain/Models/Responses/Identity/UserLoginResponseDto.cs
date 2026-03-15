namespace CMSTrain.Client.Models.Responses.Identity;

public class UserLoginResponseDto
{
    public string Token { get; set; }

    public UserDetail UserDetails { get; set; } = new();
}

public class UserDetail
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public string Email { get; set; }

    public Guid RoleId { get; set; }

    public string RoleName { get; set; }

    public string? ImageUrl { get; set; } = "";

    public string PhoneNumber { get; set; }

    public Guid CountryId { get; set; }

    public string Gender { get; set; }

    public string? Address { get; set; }

    public Guid? DesignationId { get; set; }
    
    public Guid? OrganizationId { get; set; }

    public string? Organization { get; set; }
    
    public bool IsActive { get; set; }  
}
