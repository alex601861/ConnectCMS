using Microsoft.AspNetCore.Components.Forms;

namespace CMSTrain.Client.Models.Requests.Organization;

public class CreateOrganizationDto
{
    public string Name { get; set; }

    public string? Address { get; set; }

    public string? Description { get; set; }

    public IBrowserFile? ImageUrl { get; set; }
}
