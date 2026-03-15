namespace CMSTrain.Client.Models.Requests.Questionnaires;

public class QuestionnaireDto
{
    public Guid TrainingInspectionId { get; set; }
    
    public string? FileUrl { get; set; }
}