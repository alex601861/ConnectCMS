namespace CMSTrain.Application.DTOs.Subordinate;

public class CreateSubordinateDto
{
    public Guid TrainingId { get; set; }

    public SubordinateDetails SubordinateDetails { get; set; }
}