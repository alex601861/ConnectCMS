using CMSTrain.Application.DTOs.Candidate;

namespace CMSTrain.Application.DTOs.TrainingCandidate;

public class GetApprovedCandidateDetailsDto : GetCandidateDetailsDto
{
    public Guid TrainingCandidateId { get; set; }
    
    public string? ApprovedDate { get; set; }
    
    public string RequestedDate { get; set; }
    
    public string? ActionDate { get; set; }
    
    public bool IsSelfRegistered { get; set; }

    public bool IsClientRequestRegistered { get; set; }

    public bool IsAdminRegistered { get; set; }
}