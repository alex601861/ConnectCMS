namespace CMSTrain.Client.Models.Responses.Subordinate;

public class GetSubordinateDto
{
    public Guid Id { get; set; }

    public Guid TrainingCandidateId { get; set; }

    public string Type { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public string ContactNumber { get; set; }
}