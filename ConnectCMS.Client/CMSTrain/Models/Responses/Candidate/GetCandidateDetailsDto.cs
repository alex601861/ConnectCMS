using CMSTrain.Client.Models.Responses.Organization;

namespace CMSTrain.Client.Models.Responses.Candidate;

public class GetCandidateDetailsDto
{
    public Guid Id { get; set; }
    
    public string? ImageUrl { get; set; } = "";

    public string Name { get; set; } = "";
    
    public string PhoneNumber { get; set; }
    
    public string EmailAddress { get; set; }
    
    public string Gender { get; set; }
    
    public Guid? DesignationId { get; set; }
    
    public string? Designation  { get; set; } 

    public GetOrganizationDto? Organization { get; set; } = new();
}