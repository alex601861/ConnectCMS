using CMSTrain.Application.DTOs.Identity;

namespace CMSTrain.Application.DTOs.ClientOrganization;

public class RegisterClientAdminDto : RegisterDto
{
    public Guid OrganizationId { get; set; }
}