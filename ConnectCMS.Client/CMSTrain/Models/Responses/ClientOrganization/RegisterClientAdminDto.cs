using CMSTrain.Client.Models.Requests.Identity;

namespace CMSTrain.Client.Models.Responses.ClientOrganization;

public class RegisterClientAdminDto : RegisterDto
{
    public Guid OrganizationId { get; set; }
    
    public string Image { get; set; }
}