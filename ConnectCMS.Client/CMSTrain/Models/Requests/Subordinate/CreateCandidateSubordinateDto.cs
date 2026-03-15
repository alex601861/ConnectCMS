namespace CMSTrain.Client.Models.Requests.Subordinate;

public class CreateCandidateSubordinateDto
{
    public Guid TrainingId { get; set; }

    public SubordinateDetails SubordinateDetails { get; set; } = new();
}