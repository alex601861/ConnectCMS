using CMSTrain.Client.Models.Responses.Candidate;

namespace CMSTrain.Client.Models.Responses.ClientOrganization;

public class GetClientOrganizationUsersDto : GetCandidateDetailsDto
{
    public Guid? TrainingCandidateId { get; set; }

    public string RequestedDate { get; set; }
    
    public string? ActionDate { get; set; }
    
    public string? Remarks { get; set; }
}