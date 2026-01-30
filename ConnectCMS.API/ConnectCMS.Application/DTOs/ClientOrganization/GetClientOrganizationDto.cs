using CMSTrain.Application.DTOs.Organization;

namespace CMSTrain.Application.DTOs.ClientOrganization;

public class GetClientOrganizationDto : GetOrganizationDto
{
    public GetClientAdminDto? Admin { get; set; }
    
    public int UserCount { get; set; }
}