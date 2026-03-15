namespace CMSTrain.Client.Models.Requests.TrainingCandidate;

public class AssignCandidatesDto
{
    public List<Guid>? CandidateIds { get; set; }

    public Guid TrainingId { get; set; }
}