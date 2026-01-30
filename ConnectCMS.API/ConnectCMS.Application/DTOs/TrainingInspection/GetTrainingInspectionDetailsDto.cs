namespace CMSTrain.Application.DTOs.TrainingInspection;

public class GetTrainingInspectionDetailsDto
{
    public Guid TrainingInspectionId { get; set; }

    public Guid TrainingId { get; set; }
    
    public Guid InspectionId { get; set; }

    public Guid QuestionnaireId { get; set; }
    
    public int Phases { get; set; }
}