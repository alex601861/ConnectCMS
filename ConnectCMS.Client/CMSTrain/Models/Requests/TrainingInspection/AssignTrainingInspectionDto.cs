namespace CMSTrain.Client.Models.Requests.TrainingInspection;

public class AssignTrainingInspectionDto
{
    public Guid TrainingId { get; set; }
    
    public List<Guid> InspectionId { get; set; }
}