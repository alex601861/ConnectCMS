namespace CMSTrain.Application.DTOs.Questionnaires;

// Generic class to be inherited during Excel Upload or Form Submission
public class QuestionnaireDto
{
    public Guid TrainingInspectionId { get; set; }
    
    public string? FileUrl { get; set; }
}