using CMSTrain.Client.Models.Responses.Training;

namespace CMSTrain.Client.Models.Responses.TrainingCandidate;

public class GetAllTrainingRequestsForCandidate
{
    public Guid TrainingCandidateId { get; set; }
    
    public string Action { get; set; }

    public string RequestedDate { get; set; }

    public string? ActionDate { get; set; }

    public string? Remarks { get; set; }
     
    public bool IsSelfRequested { get; set; }

    public bool IsOrganizationRequested { get; set; }
    
    public bool IsAdminRequested { get; set; }
    
    public Guid? OrganizationId { get; set; }
    
    public string? Organization { get; set; }
    
    public GetTrainingDto TrainingDetails { get; set; } = new();
}