namespace CMSTrain.Application.DTOs.ClientOrganization;

public class GetClientAdminDto
{
    public Guid Id { get; set; }
    
    public string? ImageUrl { get; set; }

    public string Name { get; set; }
    
    public string PhoneNumber { get; set; }
    
    public string EmailAddress { get; set; }
}