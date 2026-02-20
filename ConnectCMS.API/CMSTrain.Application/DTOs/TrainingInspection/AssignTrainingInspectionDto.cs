namespace CMSTrain.Application.DTOs.TrainingInspection;

public class AssignTrainingInspectionDto
{
    public Guid TrainingId { get; set; }
    
    public List<Guid> InspectionId { get; set; }
}