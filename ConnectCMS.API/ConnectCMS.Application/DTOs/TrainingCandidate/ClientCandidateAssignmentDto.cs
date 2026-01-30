namespace CMSTrain.Application.DTOs.TrainingCandidate;

public class ClientCandidateAssignmentDto : SelfCandidateAssignmentDto
{
    public List<Guid> CandidateIds { get; set; }
}