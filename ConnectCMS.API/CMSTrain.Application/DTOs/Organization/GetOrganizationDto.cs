namespace CMSTrain.Application.DTOs.Organization;

public class GetOrganizationDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Address { get; set; }

    public string Description { get; set; }

    public string ImageUrl { get; set; }

    public bool IsActive { get; set;}
}
