namespace CMSTrain.Client.Models.Requests.TrainingCandidate;

public class ClientCandidateAssignmentDto : SelfCandidateAssignmentDto
{
    public List<Guid> CandidateIds { get; set; }
}