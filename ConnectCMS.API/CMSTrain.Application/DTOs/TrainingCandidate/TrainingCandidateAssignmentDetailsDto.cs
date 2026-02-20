namespace CMSTrain.Application.DTOs.TrainingCandidate;

public class TrainingCandidateAssignmentDetailsDto
{
    public Guid TrainingCandidateId { get; set; }

    public Guid TrainingId { get; set; }
    
    public Guid CandidateId { get; set; }

    public bool IsActionCompleted { get; set; }

    public bool IsApproved { get; set; }

    public bool IsSelfRegistered { get; set; }

    public bool IsClientRequestRegistered { get; set; }

    public bool IsAdminRegistered { get; set; }

    public Guid? OrganizationId { get; set; }
    
    public string RequestedDate { get; set; }
    
    public string? ActionDate { get; set; }
    
    public string? Remarks { get; set; }
}
