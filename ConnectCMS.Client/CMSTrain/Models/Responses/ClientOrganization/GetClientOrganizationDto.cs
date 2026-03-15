using CMSTrain.Client.Models.Responses.Organization;

namespace CMSTrain.Client.Models.Responses.ClientOrganization;

public class GetClientOrganizationDto : GetOrganizationDto
{
    public GetClientAdminDto? Admin { get; set; }
    
    public int UserCount { get; set; }
}