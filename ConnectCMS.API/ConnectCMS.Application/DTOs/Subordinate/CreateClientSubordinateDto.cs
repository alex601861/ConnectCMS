namespace CMSTrain.Application.DTOs.Subordinate;

public class CreateClientSubordinateDto
{
    public Guid TrainingCandidateId { get; set; }
    
    public SubordinateDetails SubordinateDetails { get; set; }
}