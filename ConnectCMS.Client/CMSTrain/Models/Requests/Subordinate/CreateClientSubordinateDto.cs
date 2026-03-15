namespace CMSTrain.Client.Models.Requests.Subordinate;

public class CreateClientSubordinateDto
{
    public Guid TrainingCandidateId { get; set; }

    public SubordinateDetails SubordinateDetails { get; set; } = new();
}