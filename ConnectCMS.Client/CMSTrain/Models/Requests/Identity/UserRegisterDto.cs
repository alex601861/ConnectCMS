namespace CMSTrain.Client.Models.Requests.Identity;

public class UserRegisterDto : RegisterDto
{
    public Guid RoleId { get; set; }

    public Guid OrganizationId { get; set; }
    
    public string Image { get; set; }
}