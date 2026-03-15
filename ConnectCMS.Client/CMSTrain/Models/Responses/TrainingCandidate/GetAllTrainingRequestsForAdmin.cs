using CMSTrain.Client.Models.Responses.Training;
using CMSTrain.Client.Models.Responses.Candidate;

namespace CMSTrain.Client.Models.Responses.TrainingCandidate;

public class GetAllTrainingRequestsForAdmin
{
    public Guid TrainingCandidateId { get; set; }

    public string Action { get; set; }

    public string RequestedDate { get; set; }

    public string? ActionDate { get; set; }

    public string? Remarks { get; set; }
    
    public bool IsSelfRequested { get; set; }

    public bool IsOrganizationRequested { get; set; }

    public bool IsAdminRequested { get; set; }

    public GetCandidateDetailsDto CandidateDetails { get; set; } = new();
    
    public GetTrainingDto TrainingDetails { get; set; } = new();
}