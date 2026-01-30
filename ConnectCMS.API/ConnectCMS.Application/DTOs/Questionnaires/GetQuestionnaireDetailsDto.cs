namespace CMSTrain.Application.DTOs.Questionnaires;

public class GetQuestionnaireDetailsDto
{
    public Guid QuestionnaireId { get; set; }
    
    public Guid TrainingInspectionId { get; set; }
    
    public Guid TrainingId { get; set; }
    
    public Guid InspectionId { get; set; }
}